namespace TrainingCenter.Api.Infrastructure.Mail;

public sealed record EmailMessage(string To, string Subject, string Body);
