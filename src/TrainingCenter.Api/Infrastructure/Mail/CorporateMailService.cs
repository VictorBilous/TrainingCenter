using Microsoft.Extensions.Options;

namespace TrainingCenter.Api.Infrastructure.Mail;

/// <summary>
/// Условный корпоративный почтовый сервис. Пока идут технические работы,
/// бросает <see cref="MailServiceUnavailableException"/> (флаг CorporateMail:SimulateOutage).
/// </summary>
public sealed class CorporateMailService : IMailService
{
    private readonly IOptionsMonitor<CorporateMailOptions> _options;
    private readonly InMemoryMailbox _mailbox;

    public CorporateMailService(IOptionsMonitor<CorporateMailOptions> options, InMemoryMailbox mailbox)
    {
        _options = options;
        _mailbox = mailbox;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct)
    {
        // Имитация сетевого вызова к SMTP-серверу.
        await Task.Delay(TimeSpan.FromMilliseconds(100), ct);

        if (_options.CurrentValue.SimulateOutage)
        {
            throw new MailServiceUnavailableException(
                "Корпоративный почтовый сервер недоступен: идут технические работы.");
        }

        _mailbox.Add(message, MailChannel.Primary);
    }
}
