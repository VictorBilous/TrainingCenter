namespace TrainingCenter.Api.Infrastructure.Mail;

public sealed class MailServiceUnavailableException(string message) : Exception(message);
