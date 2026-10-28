using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Infrastructure.Data;

/// <summary>Профили пользователей в памяти (mock вместо БД).</summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private static readonly IReadOnlyDictionary<int, UserProfile> Users = new[]
    {
        new UserProfile(1, "Иван Петров", IsGraduate: false),
        new UserProfile(2, "Анна Смирнова", IsGraduate: true),
    }.ToDictionary(user => user.Id);

    public Task<UserProfile?> GetByIdAsync(int id, CancellationToken ct) =>
        Task.FromResult(Users.GetValueOrDefault(id));
}
