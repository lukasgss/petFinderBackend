using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Alerts;

public class AdoptionAlert
{
	public Guid Id { get; set; }

	public bool OnlyForScreenedProperties { get; set; }

	[Required]
	public required Point Location { get; set; }

	[MaxLength(500)]
	public string? Description { get; set; }

	[Required]
	public DateTime RegistrationDate { get; set; }

	public DateOnly? AdoptionDate { get; set; }

	[ForeignKey("PetId")]
	public virtual Pet Pet { get; set; } = null!;

	public Guid PetId { get; set; }

	[ForeignKey("UserId")]
	public virtual User User { get; set; } = null!;

	public Guid UserId { get; set; }
}