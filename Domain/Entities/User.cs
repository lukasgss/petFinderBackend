using System.ComponentModel.DataAnnotations;
using Domain.Entities.Alerts.Notifications;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<Guid>
{
	[Required, MaxLength(255)]
	public string FullName { get; set; } = null!;

	[MaxLength(180)]
	public string Image { get; set; } = null!;

	public ICollection<MissingAlertComment> MissingAlertComments { get; set; } = null!;
	public ICollection<FoundAnimalAlertNotifications> FoundAnimalAlertNotifications { get; set; } = null!;
}