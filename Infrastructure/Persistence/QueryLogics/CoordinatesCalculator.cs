using System.Device.Location;
using System.Linq.Expressions;
using Domain.Entities.Alerts;

namespace Infrastructure.Persistence.QueryLogics;

public static class CoordinatesCalculator
{
    public static Expression<Func<AdoptionAlert, bool>> IsWithinRadiusDistance(
        GeoCoordinate sourceCoordinates, double radiusDistanceInKm)
    {
        // Uses the haversine formula to calculate the great-circle distance between two points
        // on a sphere given their latitudes and longitudes, assuming a spherical Earth
        const int earthRadiusInKm = 6371;

        return alert =>
            earthRadiusInKm * Math.Acos(
                Math.Cos(Math.PI / 180 * sourceCoordinates.Latitude) *
                Math.Cos(Math.PI / 180 * alert.LocationLatitude) *
                Math.Cos(Math.PI / 180 * alert.LocationLongitude - Math.PI / 180 * sourceCoordinates.Longitude) +
                Math.Sin(Math.PI / 180 * sourceCoordinates.Latitude) *
                Math.Sin(Math.PI / 180 * alert.LocationLatitude)
            ) <= radiusDistanceInKm;
    }
}