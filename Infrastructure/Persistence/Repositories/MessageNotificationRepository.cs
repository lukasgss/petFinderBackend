using Application.Services.General.MessageNotifications;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MessageNotificationRepository : IMessageNotificationRepository
{
	private readonly AppDbContext _dbContext;

	public MessageNotificationRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<int> GetAmountOfNotificationsForUserAsync(Guid userId)
	{
		return await _dbContext.UserMessages
			.Where(message => message.ReceiverId == userId && !message.HasBeenRead)
			.CountAsync();
	}
}