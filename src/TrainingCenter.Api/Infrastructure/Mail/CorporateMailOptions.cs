namespace TrainingCenter.Api.Infrastructure.Mail;

public sealed class CorporateMailOptions
{
    public const string SectionName = "CorporateMail";

    /// <summary>Имитировать недоступность корпоративного сервера (технические работы).</summary>
    public bool SimulateOutage { get; set; } = true;
}
