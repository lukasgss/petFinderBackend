namespace Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;

public class CreateAlertCommentRequest
{
	public string Content { get; set; } = null!;
	public Guid AlertId { get; set; }
}