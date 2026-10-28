using TrainingCenter.Api.Contracts;
using TrainingCenter.Api.Domain;

namespace TrainingCenter.Api.Services.Leads;

public interface ILeadService
{
    /// <summary>Регистрирует заявку и доставляет её специалистам (с fallback на резервный канал).</summary>
    Task<Lead> SubmitAsync(LeadRequest request, CancellationToken ct);
}
