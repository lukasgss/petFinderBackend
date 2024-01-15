namespace Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;

public class AlertCommentResponse
{
	public Guid Id { get; set; }
	public string Content { get; set; } = null!;
	public Guid AlertId { get; set; }
	public Guid CommentOwnerId { get; set; }
	public DateTime Date { get; set; }
}