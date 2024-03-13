using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Domain.Entities.Alerts;

public class MissingAlert
{
	public Guid Id { get; set; }

	[Required]
	public DateTime RegistrationDate { get; set; }

	[Required]
	public required Point Location { get; set; }

	[MaxLength(500)]
	public string? Description { get; set; }

	public DateOnly? RecoveryDate { get; set; }

	[Required, ForeignKey("PetId")]
	public virtual Pet Pet { get; set; } = null!;

	public Guid PetId { get; set; }

	[Required, ForeignKey("UserId")]
	public virtual User User { get; set; } = null!;

	public Guid UserId { get; set; }
}