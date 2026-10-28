using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Infrastructure.Data;

/// <summary>Каталог курсов в памяти (mock вместо БД).</summary>
public sealed class InMemoryCourseRepository : ICourseRepository
{
    private readonly IReadOnlyDictionary<int, Course> _courses;

    public InMemoryCourseRepository(TimeProvider timeProvider)
    {
        // Даты старта считаем от «сегодня», чтобы в демо-данных всегда были горящие группы.
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

        _courses = new[]
        {
            new Course(1, "C# для начинающих", 30_000m, today.AddDays(3)),
            new Course(2, "ASP.NET Core Web API", 45_000m, today.AddDays(14)),
            new Course(3, "Docker и Kubernetes", 38_000m, today.AddDays(1)),
            new Course(4, "SQL для аналитиков", 25_000m, today.AddDays(5)),
        }.ToDictionary(course => course.Id);
    }

    public Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Course>>(_courses.Values.ToList());

    public Task<IReadOnlyList<Course>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<Course>>(
            ids.Where(_courses.ContainsKey).Select(id => _courses[id]).ToList());
}
