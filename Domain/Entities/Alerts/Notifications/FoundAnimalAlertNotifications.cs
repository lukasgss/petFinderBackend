using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Alerts.Notifications;

public class FoundAnimalAlertNotifications
{
	public long Id { get; set; }

	[Required]
	public required DateTime TimeStampUtc { get; set; }

	[Required]
	public virtual required ICollection<User> Users { get; set; } = null!;

	[Required, ForeignKey("FoundAnimalAlertId")]
	public virtual required FoundAnimalAlert FoundAnimalAlert { get; set; } = null!;

	public virtual required Guid FoundAnimalAlertId { get; set; }
}