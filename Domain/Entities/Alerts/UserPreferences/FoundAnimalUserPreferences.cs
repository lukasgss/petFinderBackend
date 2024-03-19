using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Alerts.UserPreferences;

public class FoundAnimalUserPreferences
{
	public required Guid Id { get; set; }
	public Point? Location { get; set; }
	public double? RadiusDistanceInKm { get; set; }
	public List<Gender>? Genders { get; set; }
	public List<Age>? Ages { get; set; }
	public virtual List<Size>? Sizes { get; set; }
	public virtual List<Species>? Species { get; set; }
	public virtual List<Breed>? Breeds { get; set; }
	public virtual List<Color>? Colors { get; set; }

	[Required, ForeignKey("UserId")]
	public virtual required User User { get; set; } = null!;

	public required Guid UserId { get; set; }
}