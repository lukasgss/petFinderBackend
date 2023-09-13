using Application.Common.Interfaces.Providers;

namespace Application.Common.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow() => DateTime.UtcNow;
    public DateOnly DateOnlyNow() => DateOnly.FromDateTime(DateTime.Now);
}