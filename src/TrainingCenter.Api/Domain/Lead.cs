namespace TrainingCenter.Api.Domain;

/// <summary>Заявка на обратный звонок.</summary>
public sealed record Lead(
    Guid Id,
    string Name,
    string Phone,
    string Email,
    ClientType ClientType,
    DateTimeOffset CreatedAt);
