using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Pagination;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class AdoptionAlertCommentRepository : GenericRepository<AdoptionAlertComment>, IAdoptionAlertCommentRepository
{
	private readonly AppDbContext _dbContext;

	public AdoptionAlertCommentRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<AdoptionAlertComment?> GetByIdAsync(Guid commentId)
	{
		return await _dbContext.AdoptionAlertComments
			.Include(comment => comment.User)
			.SingleOrDefaultAsync(comment => comment.Id == commentId);
	}

	public async Task<PagedList<AdoptionAlertComment>> GetCommentsByAlertIdAsync(
		Guid alertId, int pageNumber, int pageSize)
	{
		var query = _dbContext.AdoptionAlertComments
			.AsNoTracking()
			.Include(comment => comment.User)
			.Where(comment => comment.AdoptionAlertId == alertId);

		return await PagedList<AdoptionAlertComment>.ToPagedListAsync(query, pageNumber, pageSize);
	}
}