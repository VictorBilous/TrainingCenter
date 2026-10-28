namespace TrainingCenter.Api.Services.Pricing;

/// <summary>Скидка, которую предлагает правило.</summary>
/// <param name="Name">Название скидки для отображения клиенту.</param>
/// <param name="Percent">Размер скидки в процентах.</param>
public sealed record Discount(string Name, decimal Percent);
