using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Pricing;

/// <summary>Расчёт стоимости корзины с учётом скидок.</summary>
public interface IPriceCalculator
{
    CartPrice Calculate(IReadOnlyCollection<Course> courses, UserProfile user);
}
