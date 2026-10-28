namespace TrainingCenter.Tests;

/// <summary>Часы с фиксированным «сейчас», чтобы тесты не зависели от реальной даты.</summary>
internal sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;

    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
