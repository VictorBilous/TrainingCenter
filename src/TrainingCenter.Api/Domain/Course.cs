namespace TrainingCenter.Api.Domain;

/// <summary>Курс учебного центра.</summary>
/// <param name="Id">Идентификатор курса.</param>
/// <param name="Title">Название курса.</param>
/// <param name="Price">Базовая стоимость без скидок, ₽.</param>
/// <param name="StartDate">Дата старта группы.</param>
public sealed record Course(int Id, string Title, decimal Price, DateOnly StartDate);
