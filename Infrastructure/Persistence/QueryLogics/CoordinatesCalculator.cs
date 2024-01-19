using System.Linq.Expressions;
using Domain.Entities.Alerts;
using GeoCoordinatePortable;

namespace Infrastructure.Persistence.QueryLogics;

public static class CoordinatesCalculator
{
	public static Expression<Func<AdoptionAlert, bool>> AdoptionAlertIsWithinRadiusDistance(
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

	public static Expression<Func<MissingAlert, bool>> MissingAlertIsWithinRadiusDistance(
		GeoCoordinate sourceCoordinates, double radiusDistanceInKm)
	{
		// Uses the haversine formula to calculate the great-circle distance between two points
		// on a sphere given their latitudes and longitudes, assuming a spherical Earth
		const int earthRadiusInKm = 6371;

		return alert =>
			earthRadiusInKm * Math.Acos(
				Math.Cos(Math.PI / 180 * sourceCoordinates.Latitude) *
				Math.Cos(Math.PI / 180 * alert.LastSeenLocationLatitude) *
				Math.Cos(Math.PI / 180 * alert.LastSeenLocationLongitude -
				         Math.PI / 180 * sourceCoordinates.Longitude) +
				Math.Sin(Math.PI / 180 * sourceCoordinates.Latitude) *
				Math.Sin(Math.PI / 180 * alert.LastSeenLocationLatitude)
			) <= radiusDistanceInKm;
	}
}