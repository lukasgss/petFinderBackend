using Application.Common.Interfaces.Entities.MissingAlertComments;
using Application.Common.Pagination;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MissingAlertCommentRepository : GenericRepository<MissingAlertComment>, IMissingAlertCommentRepository
{
	private readonly AppDbContext _dbContext;

	public MissingAlertCommentRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext;
	}


	public async Task<MissingAlertComment?> GetByIdAsync(Guid commentId)
	{
		return await _dbContext.MissingAlertComments
			.SingleOrDefaultAsync(comment => comment.Id == commentId);
	}

	public async Task<PagedList<MissingAlertComment>> GetCommentsByAlertIdAsync(Guid alertId, int pageNumber,
		int pageSize)
	{
		var query = _dbContext.MissingAlertComments
			.Where(comment => comment.MissingAlertId == alertId);

		return await PagedList<MissingAlertComment>.ToPagedListAsync(query, pageNumber, pageSize);
	}
}