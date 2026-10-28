namespace TrainingCenter.Api.Services.Pricing;

/// <summary>Итоговая стоимость корзины.</summary>
/// <param name="Items">Расчёт по каждому курсу.</param>
/// <param name="TotalBeforeDiscount">Сумма без скидок, ₽.</param>
/// <param name="TotalDiscount">Сумма скидок, ₽.</param>
/// <param name="Total">Итого к оплате, ₽.</param>
public sealed record CartPrice(
    IReadOnlyList<CartItemPrice> Items,
    decimal TotalBeforeDiscount,
    decimal TotalDiscount,
    decimal Total);

/// <summary>Стоимость одного курса в корзине.</summary>
/// <param name="CourseId">Идентификатор курса.</param>
/// <param name="Title">Название курса.</param>
/// <param name="BasePrice">Цена без скидки, ₽.</param>
/// <param name="AppliedDiscount">Название применённой скидки или <c>null</c>, если скидки нет.</param>
/// <param name="DiscountPercent">Размер применённой скидки, %.</param>
/// <param name="DiscountAmount">Размер применённой скидки, ₽.</param>
/// <param name="FinalPrice">Цена со скидкой, ₽.</param>
public sealed record CartItemPrice(
    int CourseId,
    string Title,
    decimal BasePrice,
    string? AppliedDiscount,
    decimal DiscountPercent,
    decimal DiscountAmount,
    decimal FinalPrice);
