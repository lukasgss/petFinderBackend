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

	public Gender? Gender { get; set; }

	[ForeignKey("SpeciesId")]
	public virtual Species? Species { get; set; }

	public int? SpeciesId { get; set; }

	[ForeignKey("BreedId")]
	public virtual Breed? Breed { get; set; }

	public int? BreedId { get; set; }

	[Required, ForeignKey("UserId")]
	public virtual required User User { get; set; } = null!;

	public required Guid UserId { get; set; }

	[ForeignKey("AgeId")]
	public virtual Age? Age { get; set; }

	public int? AgeId { get; set; }

	public virtual required ICollection<Color> Colors { get; set; } = null!;
}