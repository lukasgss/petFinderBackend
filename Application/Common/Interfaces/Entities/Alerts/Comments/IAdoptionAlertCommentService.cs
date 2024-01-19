using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Paginated;

namespace Application.Common.Interfaces.Entities.Alerts.Comments;

public interface IAdoptionAlertCommentService
{
	Task<AlertCommentResponse> GetCommentByIdAsync(Guid commentId);
	Task<PaginatedEntity<AlertCommentResponse>> ListCommentsAsync(Guid alertId, int pageNumber, int pageSize);
	Task<AlertCommentResponse> PostCommentAsync(CreateAlertCommentRequest alertRequest, Guid userId, Guid routeId);
	Task DeleteCommentAsync(Guid commentId, Guid userId);
}