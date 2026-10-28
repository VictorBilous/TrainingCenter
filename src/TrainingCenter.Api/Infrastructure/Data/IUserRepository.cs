using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Infrastructure.Data;

public interface IUserRepository
{
    Task<UserProfile?> GetByIdAsync(int id, CancellationToken ct);
}
