namespace TrainingCenter.Api.Domain;

/// <summary>Профиль пользователя сайта.</summary>
/// <param name="Id">Идентификатор пользователя.</param>
/// <param name="FullName">ФИО.</param>
/// <param name="IsGraduate">Пользователь уже окончил один из курсов центра.</param>
public sealed record UserProfile(int Id, string FullName, bool IsGraduate);
