using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Pagination;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Infrastructure.Persistence.Repositories;

public class UserMessageRepository : GenericRepository<UserMessage>, IUserMessageRepository
{
    private readonly AppDbContext _dbContext;

    public UserMessageRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserMessage?> GetByIdAsync(long messageId, Guid receiverId)
    {
        return await _dbContext.UserMessages
            .Include(message => message.Sender)
            .Include(message => message.Receiver)
            .SingleOrDefaultAsync(message => message.Id == messageId 
                                             && message.Receiver.Id == receiverId);
    }

    public async Task<PagedList<UserMessage>> GetAllAsync(Guid senderId, Guid receiverId, int pageNumber, int pageSize)
    {
        var query =_dbContext.UserMessages
            .Include(message => message.Sender)
            .Include(message => message.Receiver)
            .Where(message => message.Sender.Id == senderId
                              && message.Receiver.Id == receiverId)
            .OrderByDescending(message => message.TimeStamp);

        return await PagedList<UserMessage>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task ReadAllAsync(Guid senderId, Guid receiverId)
    {
        await _dbContext.UserMessages
            .Where(message => message.Sender.Id == senderId && message.Receiver.Id == receiverId)
            .UpdateAsync(message => new { HasBeenRead = true });
    }
}