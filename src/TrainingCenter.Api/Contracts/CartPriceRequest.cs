using System.ComponentModel.DataAnnotations;

namespace TrainingCenter.Api.Contracts;

/// <summary>Запрос на расчёт стоимости корзины.</summary>
public sealed class CartPriceRequest
{
    /// <summary>Идентификатор пользователя (1 — обычный, 2 — выпускник).</summary>
    /// <example>2</example>
    [Required(ErrorMessage = "Укажите пользователя")]
    public int? UserId { get; init; }

    /// <summary>Курсы в корзине.</summary>
    /// <example>[1, 2, 3, 4]</example>
    [Required(ErrorMessage = "Корзина пуста")]
    [MinLength(1, ErrorMessage = "Корзина пуста")]
    public int[] CourseIds { get; init; } = [];
}
