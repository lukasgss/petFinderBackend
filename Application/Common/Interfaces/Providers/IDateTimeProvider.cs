namespace Application.Common.Interfaces.Providers;

public interface IDateTimeProvider
{
    DateTime Now();
    DateTime UtcNow();
    DateOnly DateOnlyNow();
}