using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace TrainingCenter.Api.Infrastructure.Mail;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MailChannel
{
    Primary,
    Backup
}

public sealed record SentEmail(EmailMessage Message, MailChannel Channel, DateTimeOffset SentAt);

/// <summary>
/// «Отправленные» письма в памяти — замена реальному SMTP.
/// Singleton на всё приложение, поэтому коллекция потокобезопасная.
/// </summary>
public sealed class InMemoryMailbox
{
    private readonly ConcurrentQueue<SentEmail> _sent = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryMailbox(TimeProvider timeProvider) => _timeProvider = timeProvider;

    public void Add(EmailMessage message, MailChannel channel) =>
        _sent.Enqueue(new SentEmail(message, channel, _timeProvider.GetUtcNow()));

    public IReadOnlyCollection<SentEmail> GetAll() => _sent.ToArray();
}
