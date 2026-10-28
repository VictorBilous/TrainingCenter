using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Pricing;

/// <summary>
/// Правило скидки. Каждое правило отвечает только за своё условие;
/// выбор одной (максимальной) скидки делает <see cref="PriceCalculator"/>.
/// Новая акция = новый класс + регистрация в DI, калькулятор не меняется.
/// </summary>
public interface IDiscountRule
{
    /// <returns>Скидка или <c>null</c>, если правило к курсу не применимо.</returns>
    Discount? Evaluate(Course course, UserProfile user, DateOnly today);
}
