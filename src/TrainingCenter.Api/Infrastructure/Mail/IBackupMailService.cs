namespace TrainingCenter.Api.Infrastructure.Mail;

/// <summary>
/// Резервный канал отправки почты (например, внешний SMTP), не зависящий от корпоративных серверов.
/// Отдельный интерфейс, чтобы DI однозначно различал основной и резервный сервисы.
/// </summary>
public interface IBackupMailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct);
}
