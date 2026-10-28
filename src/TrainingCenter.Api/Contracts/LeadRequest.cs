using System.ComponentModel.DataAnnotations;
using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Contracts;

/// <summary>Заявка с формы «Обратный звонок».</summary>
public sealed class LeadRequest
{
    /// <summary>Имя клиента.</summary>
    /// <example>Иван Петров</example>
    [Required(ErrorMessage = "Укажите имя")]
    [StringLength(100, ErrorMessage = "Имя не должно быть длиннее 100 символов")]
    public string Name { get; init; } = string.Empty;

    /// <summary>Телефон: 10–15 цифр, допускаются +, пробелы, дефисы и скобки.</summary>
    /// <example>+7 (999) 123-45-67</example>
    [Required(ErrorMessage = "Укажите телефон")]
    [RegularExpression(@"^(?=(?:\D*\d){10,15}\D*$)\+?[\d\s\-()]+$", ErrorMessage = "Некорректный номер телефона")]
    public string Phone { get; init; } = string.Empty;

    /// <summary>Email клиента.</summary>
    /// <example>ivan.petrov@example.com</example>
    // Стандартный [EmailAddress] пропускает адреса вида "a@b", поэтому требуем домен с точкой.
    [Required(ErrorMessage = "Укажите email")]
    [StringLength(254, ErrorMessage = "Email слишком длинный")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Некорректный email")]
    public string Email { get; init; } = string.Empty;

    /// <summary>Тип клиента: Individual — физ. лицо, LegalEntity — юр. лицо.</summary>
    /// <example>Individual</example>
    [Required(ErrorMessage = "Укажите тип клиента")]
    [EnumDataType(typeof(ClientType), ErrorMessage = "Недопустимый тип клиента")]
    public ClientType? ClientType { get; init; }
}
