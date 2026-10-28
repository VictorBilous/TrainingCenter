namespace TrainingCenter.Api.Infrastructure.Mail;

/// <summary>Основной (корпоративный) почтовый сервис.</summary>
public interface IMailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct);
}
