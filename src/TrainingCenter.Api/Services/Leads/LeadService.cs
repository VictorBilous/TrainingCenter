using Microsoft.Extensions.Options;
using TrainingCenter.Api.Contracts;
using TrainingCenter.Api.Domain;
using TrainingCenter.Api.Infrastructure.Mail;

namespace TrainingCenter.Api.Services.Leads;

public sealed class LeadService : ILeadService
{
    private readonly IMailService _primaryMail;
    private readonly IBackupMailService _backupMail;
    private readonly LeadRoutingOptions _routing;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<LeadService> _logger;

    public LeadService(
        IMailService primaryMail,
        IBackupMailService backupMail,
        IOptions<LeadRoutingOptions> routing,
        TimeProvider timeProvider,
        ILogger<LeadService> logger)
    {
        _primaryMail = primaryMail;
        _backupMail = backupMail;
        _routing = routing.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<Lead> SubmitAsync(LeadRequest request, CancellationToken ct)
    {
        var lead = new Lead(
            Guid.NewGuid(),
            request.Name.Trim(),
            request.Phone.Trim(),
            request.Email.Trim(),
            request.ClientType!.Value,
            _timeProvider.GetUtcNow());

        try
        {
            // Основной сервис может не упасть, а зависнуть — ограничиваем ожидание таймаутом.
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            timeoutCts.CancelAfter(_routing.PrimaryTimeout);

            await _primaryMail.SendAsync(ComposeEmail(lead, _routing.PrimaryRecipient), timeoutCts.Token);
            _logger.LogInformation("Lead {LeadId} sent via primary mail service", lead.Id);
        }
        catch (Exception ex) when (!ct.IsCancellationRequested)
        {
            // Любой сбой основного сервиса (исключение или таймаут) клиент замечать не должен:
            // переотправляем заявку на резервный ящик профильного специалиста.
            // Если же запрос отменил сам клиент (ct отменён) — фильтр не сработает, fallback не нужен.
            var backupRecipient = _routing.GetBackupRecipient(lead.ClientType);
            _logger.LogWarning(ex,
                "Primary mail service failed for lead {LeadId}. Redirecting to backup recipient {BackupRecipient}",
                lead.Id, backupRecipient);

            await _backupMail.SendAsync(ComposeEmail(lead, backupRecipient), ct);
        }

        return lead;
    }

    private static EmailMessage ComposeEmail(Lead lead, string recipient)
    {
        var clientType = lead.ClientType == ClientType.LegalEntity ? "Юридическое лицо" : "Физическое лицо";

        return new EmailMessage(
            To: recipient,
            Subject: $"Заявка на обратный звонок от {lead.Name}",
            Body: $"""
                Имя: {lead.Name}
                Телефон: {lead.Phone}
                Email: {lead.Email}
                Тип клиента: {clientType}
                Создана (UTC): {lead.CreatedAt:dd.MM.yyyy HH:mm}
                ID заявки: {lead.Id}
                """);
    }
}
