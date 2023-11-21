using Application.Services.General.Notifications;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _dbContext;

    public NotificationRepository(AppDbContext dbContext)
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