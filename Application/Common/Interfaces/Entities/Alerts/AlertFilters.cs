namespace Application.Common.Interfaces.Entities.Alerts;

public static class AlertFilters
{
    public static bool HasGeoFilters(BaseAlertFilters filters)
    {
        return filters.Latitude != 0 && filters.Longitude != 0 && filters.RadiusDistanceInKm != 0;
    }
}