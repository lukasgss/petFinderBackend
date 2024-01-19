using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Alerts.Comments;

public interface IAdoptionAlertCommentRepository : IGenericRepository<AdoptionAlertComment>
{
	Task<AdoptionAlertComment?> GetByIdAsync(Guid commentId);
	Task<PagedList<AdoptionAlertComment>> GetCommentsByAlertIdAsync(Guid alertId, int pageNumber, int pageSize);
}