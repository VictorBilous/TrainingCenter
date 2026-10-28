using TrainingCenter.Api.Infrastructure.Data;

namespace TrainingCenter.Api.Services.Pricing;

/// <summary>Загружает данные корзины (I/O, async) и передаёт их в калькулятор (чистая логика).</summary>
public sealed class CartService : ICartService
{
    private readonly IUserRepository _users;
    private readonly ICourseRepository _courses;
    private readonly IPriceCalculator _calculator;

    public CartService(IUserRepository users, ICourseRepository courses, IPriceCalculator calculator)
    {
        _users = users;
        _courses = courses;
        _calculator = calculator;
    }

    public async Task<CartPrice?> CalculateAsync(int userId, IReadOnlyCollection<int> courseIds, CancellationToken ct)
    {
        // Статус выпускника берём из профиля, а не из запроса: иначе скидку мог бы «заказать» любой клиент.
        var user = await _users.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return null;
        }

        var distinctIds = courseIds.Distinct().ToArray();
        var courses = await _courses.GetByIdsAsync(distinctIds, ct);
        if (courses.Count != distinctIds.Length)
        {
            return null;
        }

        return _calculator.Calculate(courses, user);
    }
}
