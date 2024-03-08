using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities.Alerts.UserPreferences;

public class FoundAnimalUserPreferences
{
	public required Guid Id { get; set; }

	[Column(TypeName = "decimal(6, 3)")]
	public double? FoundLocationLatitude { get; set; }

	[Column(TypeName = "decimal(6, 3)")]
	public double? FoundLocationLongitude { get; set; }

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

	public virtual required ICollection<Color> Colors { get; set; } = null!;
}