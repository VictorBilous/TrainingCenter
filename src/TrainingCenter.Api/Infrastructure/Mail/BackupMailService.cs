namespace TrainingCenter.Api.Infrastructure.Mail;

/// <summary>Резервный канал отправки (имитация внешнего SMTP).</summary>
public sealed class BackupMailService : IBackupMailService
{
    private readonly InMemoryMailbox _mailbox;
    private readonly ILogger<BackupMailService> _logger;

    public BackupMailService(InMemoryMailbox mailbox, ILogger<BackupMailService> logger)
    {
        _mailbox = mailbox;
        _logger = logger;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct)
    {
        // Имитация сетевого вызова к внешнему SMTP.
        await Task.Delay(TimeSpan.FromMilliseconds(50), ct);

        _mailbox.Add(message, MailChannel.Backup);
        _logger.LogInformation("Email sent to {Recipient} via backup channel", message.To);
    }
}
