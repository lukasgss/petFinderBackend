namespace Application.Common.Interfaces.Providers;

public interface IValueProvider
{
	DateTime Now();
	DateTime UtcNow();
	DateOnly DateOnlyNow();
	Guid NewGuid();
}