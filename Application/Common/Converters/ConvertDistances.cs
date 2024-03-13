namespace Application.Common.Converters;

public static class ConvertDistances
{
	public static double ConvertKmToMeters(double measurementInKm)
	{
		return measurementInKm * 1000;
	}
}