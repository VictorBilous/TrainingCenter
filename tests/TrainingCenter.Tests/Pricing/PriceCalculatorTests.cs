using TrainingCenter.Api.Domain;
using TrainingCenter.Api.Services.Pricing;

namespace TrainingCenter.Tests.Pricing;

public class PriceCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 10, 23);
    private static readonly UserProfile RegularUser = new(1, "Иван Петров", IsGraduate: false);
    private static readonly UserProfile Graduate = new(2, "Анна Смирнова", IsGraduate: true);

    private readonly PriceCalculator _calculator = new(
        [new HotGroupDiscountRule(), new GraduateDiscountRule()],
        new FixedTimeProvider(new DateTimeOffset(Today.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero)));

    [Theory]
    [InlineData(0, 20)]  // старт сегодня
    [InlineData(4, 20)]  // менее 5 дней — горящая группа
    [InlineData(5, 0)]   // ровно 5 дней — уже не «менее 5»
    [InlineData(30, 0)]
    [InlineData(-1, 0)]  // курс уже стартовал
    public void HotGroupDiscount_DependsOnDaysUntilStart(int daysUntilStart, decimal expectedPercent)
    {
        var course = new Course(1, "C#", 10_000m, Today.AddDays(daysUntilStart));

        var result = _calculator.Calculate([course], RegularUser);

        var item = Assert.Single(result.Items);
        Assert.Equal(expectedPercent, item.DiscountPercent);
        Assert.Equal(10_000m * (100 - expectedPercent) / 100, item.FinalPrice);
    }

    [Fact]
    public void GraduateOnHotCourse_GetsOnlyMaxDiscount_DiscountsAreNotSummed()
    {
        // Доступны обе скидки: 20% (горящая) и 30% (выпускник). Ожидаем 30%, а не 50%.
        var hotCourse = new Course(1, "C#", 10_000m, Today.AddDays(2));

        var result = _calculator.Calculate([hotCourse], Graduate);

        var item = Assert.Single(result.Items);
        Assert.Equal(GraduateDiscountRule.Name, item.AppliedDiscount);
        Assert.Equal(30m, item.DiscountPercent);
        Assert.Equal(7_000m, item.FinalPrice);
    }

    [Fact]
    public void Cart_EachCourseGetsItsOwnDiscount_AndTotalsAreSummed()
    {
        var courses = new[]
        {
            new Course(1, "C#", 30_000m, Today.AddDays(3)),         // горящая: 30 000 − 20% = 24 000
            new Course(2, "ASP.NET Core", 45_000m, Today.AddDays(14)), // без скидки: 45 000
        };

        var result = _calculator.Calculate(courses, RegularUser);

        Assert.Equal(75_000m, result.TotalBeforeDiscount);
        Assert.Equal(6_000m, result.TotalDiscount);
        Assert.Equal(69_000m, result.Total);
        Assert.Null(result.Items.Single(i => i.CourseId == 2).AppliedDiscount);
    }
}
