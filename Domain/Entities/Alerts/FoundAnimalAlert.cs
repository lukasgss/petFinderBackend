using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Alerts.Notifications;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.Alerts;

public class FoundAnimalAlert
{
	public Guid Id { get; set; }

	[MaxLength(255)]
	public string? Name { get; set; }

	[MaxLength(500)]
	public string? Description { get; set; }

	[Required, Column(TypeName = "decimal(6, 3)")]
	public double FoundLocationLatitude { get; set; }

	[Required, Column(TypeName = "decimal(6, 3)")]
	public double FoundLocationLongitude { get; set; }

	[Required]
	public DateTime RegistrationDate { get; set; }

	public DateOnly? RecoveryDate { get; set; }

	[Required, ForeignKey("SpeciesId")]
	public virtual Species Species { get; set; } = null!;

	public int SpeciesId { get; set; }

	[ForeignKey("BreedId")]
	public virtual Breed? Breed { get; set; }

	public int? BreedId { get; set; }

	[Required, ForeignKey("UserId")]
	public virtual User User { get; set; } = null!;

	public Guid UserId { get; set; }

	public Gender? Gender { get; set; }

	public virtual ICollection<Color> Colors { get; set; } = null!;
	public virtual List<FoundAnimalAlertImage> Images { get; set; } = null!;
	public virtual ICollection<FoundAnimalAlertNotifications> FoundAnimalAlertNotifications { get; set; } = null!;
}