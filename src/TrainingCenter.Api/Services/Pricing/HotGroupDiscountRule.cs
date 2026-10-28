using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Pricing;

/// <summary>«Горящая группа»: до старта курса осталось менее 5 дней — скидка 20%.</summary>
public sealed class HotGroupDiscountRule : IDiscountRule
{
    public const int DaysThreshold = 5;
    public const decimal Percent = 20m;
    public const string Name = "Горящая группа";

    public Discount? Evaluate(Course course, UserProfile user, DateOnly today)
    {
        var daysUntilStart = course.StartDate.DayNumber - today.DayNumber;

        // Уже стартовавший курс «горящим» не считаем.
        return daysUntilStart is >= 0 and < DaysThreshold
            ? new Discount(Name, Percent)
            : null;
    }
}
