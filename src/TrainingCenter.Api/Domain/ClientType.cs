namespace TrainingCenter.Api.Domain;

/// <summary>Тип клиента.</summary>
public enum ClientType
{
    // Нумерация с 1, чтобы значение по умолчанию (0) не считалось валидным типом.

    /// <summary>Физическое лицо.</summary>
    Individual = 1,

    /// <summary>Юридическое лицо.</summary>
    LegalEntity = 2
}
