using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Alerts.UserFavorites;

public class AdoptionFavorite
{
	public required Guid Id { get; set; }

	[Required, ForeignKey("UserId")]
	public virtual required User User { get; set; }

	public required Guid UserId { get; set; }

	[Required, ForeignKey("AdoptionAlertId")]
	public virtual required AdoptionAlert AdoptionAlert { get; set; }

	public required Guid AdoptionAlertId { get; set; }
}