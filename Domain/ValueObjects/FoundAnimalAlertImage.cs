using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Alerts;

namespace Domain.ValueObjects;

public class FoundAnimalAlertImage
{
	public long Id { get; set; }

	[Required, MaxLength(100)]
	public string ImageUrl { get; set; } = null!;

	[ForeignKey("FoundAnimalAlertId")]
	public virtual FoundAnimalAlert FoundAnimalAlert { get; set; } = null!;

	public Guid FoundAnimalAlertId { get; set; }
}