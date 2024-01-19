using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Alerts.Comments;

public interface IMissingAlertCommentRepository : IGenericRepository<MissingAlertComment>
{
	Task<MissingAlertComment?> GetByIdAsync(Guid commentId);
	Task<PagedList<MissingAlertComment>> GetCommentsByAlertIdAsync(Guid alertId, int pageNumber, int pageSize);
}