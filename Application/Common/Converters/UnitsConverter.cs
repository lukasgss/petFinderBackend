namespace Application.Common.Converters;

public static class UnitsConverter
{
	public static double ConvertKmToMeters(double measurementInKm)
	{
		return measurementInKm * 1000;
	}
}