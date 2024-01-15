using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Entities.Alerts;

namespace Domain.Entities;

public class MissingAlertComment
{
	public Guid Id { get; set; }

	[Required, MaxLength(500)]
	public string Content { get; set; } = null!;

	[Required]
	public DateTime Date { get; set; }

	[Required, ForeignKey("UserId")]
	public User User { get; set; } = null!;

	[Required]
	public Guid UserId { get; set; }

	[Required, ForeignKey("MissingAlertId")]
	public MissingAlert MissingAlert { get; set; } = null!;

	[Required]
	public Guid MissingAlertId { get; set; }
}