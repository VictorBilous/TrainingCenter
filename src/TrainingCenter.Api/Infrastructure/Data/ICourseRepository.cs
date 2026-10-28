using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Infrastructure.Data;

public interface ICourseRepository
{
    Task<IReadOnlyList<Course>> GetAllAsync(CancellationToken ct);

    /// <summary>Возвращает только найденные курсы; отсутствующие идентификаторы пропускаются.</summary>
    Task<IReadOnlyList<Course>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct);
}
