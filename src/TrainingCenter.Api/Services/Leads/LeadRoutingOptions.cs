using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Leads;

/// <summary>Куда отправлять заявки. Адреса вынесены в конфигурацию, а не зашиты в код.</summary>
public sealed class LeadRoutingOptions
{
    public const string SectionName = "LeadRouting";

    /// <summary>Адрес для заявок через основной почтовый сервис.</summary>
    [Required, EmailAddress]
    public string PrimaryRecipient { get; set; } = string.Empty;

    /// <summary>Резервный адрес для физических лиц.</summary>
    [Required, EmailAddress]
    public string IndividualBackupRecipient { get; set; } = string.Empty;

    /// <summary>Резервный адрес для юридических лиц.</summary>
    [Required, EmailAddress]
    public string LegalEntityBackupRecipient { get; set; } = string.Empty;

    /// <summary>Сколько ждать основной сервис, прежде чем считать его недоступным.</summary>
    public TimeSpan PrimaryTimeout { get; set; } = TimeSpan.FromSeconds(5);

    public string GetBackupRecipient(ClientType clientType) => clientType switch
    {
        ClientType.LegalEntity => LegalEntityBackupRecipient,
        ClientType.Individual => IndividualBackupRecipient,
        _ => throw new ArgumentOutOfRangeException(nameof(clientType), clientType, "Неизвестный тип клиента")
    };
}
