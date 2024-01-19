using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;

public class AlertCommentResponse
{
	public Guid Id { get; set; }
	public string Content { get; set; } = null!;
	public Guid AlertId { get; set; }
	public UserDataResponse CommentOwner { get; set; } = null!;
	public DateTime Date { get; set; }
}