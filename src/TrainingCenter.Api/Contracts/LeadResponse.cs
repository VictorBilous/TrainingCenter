namespace TrainingCenter.Api.Contracts;

/// <summary>Ответ на отправку заявки. Не раскрывает, через какой канал ушло письмо.</summary>
public sealed record LeadResponse(Guid LeadId, string Message);
