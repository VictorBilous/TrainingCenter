namespace TrainingCenter.Api.Services.Pricing;

public interface ICartService
{
    /// <returns>Расчёт корзины или <c>null</c>, если пользователь или один из курсов не найден.</returns>
    Task<CartPrice?> CalculateAsync(int userId, IReadOnlyCollection<int> courseIds, CancellationToken ct);
}
