using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Pricing;

/// <summary>
/// Считает стоимость корзины. Расчёт чисто вычислительный (без I/O),
/// поэтому метод синхронный — асинхронность здесь ничего бы не дала.
/// </summary>
public sealed class PriceCalculator : IPriceCalculator
{
    private readonly IReadOnlyCollection<IDiscountRule> _rules;
    private readonly TimeProvider _timeProvider;

    public PriceCalculator(IEnumerable<IDiscountRule> rules, TimeProvider timeProvider)
    {
        _rules = rules.ToArray();
        _timeProvider = timeProvider;
    }

    public CartPrice Calculate(IReadOnlyCollection<Course> courses, UserProfile user)
    {
        ArgumentNullException.ThrowIfNull(courses);
        ArgumentNullException.ThrowIfNull(user);

        var today = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
        var items = courses.Select(course => PriceCourse(course, user, today)).ToList();

        return new CartPrice(
            items,
            TotalBeforeDiscount: items.Sum(item => item.BasePrice),
            TotalDiscount: items.Sum(item => item.DiscountAmount),
            Total: items.Sum(item => item.FinalPrice));
    }

    private CartItemPrice PriceCourse(Course course, UserProfile user, DateOnly today)
    {
        // Скидки не суммируются: из всех применимых к курсу берём одну — максимальную.
        var bestDiscount = _rules
            .Select(rule => rule.Evaluate(course, user, today))
            .OfType<Discount>()
            .MaxBy(discount => discount.Percent);

        var percent = bestDiscount?.Percent ?? 0m;
        var discountAmount = Math.Round(course.Price * percent / 100m, 2, MidpointRounding.AwayFromZero);

        return new CartItemPrice(
            course.Id,
            course.Title,
            course.Price,
            bestDiscount?.Name,
            percent,
            discountAmount,
            FinalPrice: course.Price - discountAmount);
    }
}
