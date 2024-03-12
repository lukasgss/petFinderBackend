namespace Application.Common.Interfaces.Entities.Alerts;

public static class AlertFilters
{
	public static bool HasGeoFilters(BaseAlertFilters filters)
	{
		return filters.Latitude is not null && filters.Longitude is not null &&
		       filters.RadiusDistanceInKm is not null;
	}
}