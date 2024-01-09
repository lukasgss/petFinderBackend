namespace Application.Common.Interfaces.Entities.Alerts;

public class BaseAlertFilters
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public double RadiusDistanceInKm { get; init; }
}