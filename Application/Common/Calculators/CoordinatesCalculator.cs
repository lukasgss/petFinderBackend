using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace Application.Common.Calculators;

public static class CoordinatesCalculator
{
	private const int Srid = 4326;
	private static readonly GeometryFactory GeometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(Srid);

	public static Point CreatePointBasedOnCoordinates(double latitude, double longitude)
	{
		return GeometryFactory.CreatePoint(new Coordinate(longitude, latitude));
	}
}