using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Pricing;

/// <summary>«Выпускник»: у пользователя стоит флаг IsGraduate — скидка 30% на любой курс.</summary>
public sealed class GraduateDiscountRule : IDiscountRule
{
    public const decimal Percent = 30m;
    public const string Name = "Выпускник";

    public Discount? Evaluate(Course course, UserProfile user, DateOnly today) =>
        user.IsGraduate ? new Discount(Name, Percent) : null;
}
