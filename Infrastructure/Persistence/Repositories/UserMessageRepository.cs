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

    public async Task<UserMessage?> GetByIdAsync(long messageId, Guid userId)
    {
        return await _dbContext.UserMessages
            .Include(message => message.Sender)
            .Include(message => message.Receiver)
            .SingleOrDefaultAsync(message => message.Id == messageId
                                             && !message.HasBeenDeleted
                                             && (message.Receiver.Id == userId
                                                 || message.Sender.Id == userId));
    }

    public async Task<PagedList<UserMessage>> GetAllFromUserAsync(
        Guid senderId, Guid receiverId, int pageNumber, int pageSize)
    {
        var query = _dbContext.UserMessages
            .Include(message => message.Sender)
            .Include(message => message.Receiver)
            .AsNoTracking()
            .Select(message => new UserMessage
            {
                Id = message.Id,
                Content = message.Content,
                TimeStampUtc = message.TimeStampUtc,
                HasBeenRead = message.HasBeenRead,
                HasBeenEdited = message.HasBeenEdited,
                HasBeenDeleted = message.HasBeenDeleted,
                Sender = new User
                {
                    Id = message.Sender.Id,
                    Email = message.Sender.Email,
                    FullName = message.Sender.FullName
                },
                Receiver = new User
                {
                    Id = message.Receiver.Id,
                    Email = message.Receiver.Email,
                    FullName = message.Receiver.FullName
                }
            })
            .Where(message => message.Sender.Id == senderId
                              && !message.HasBeenDeleted
                              && message.Receiver.Id == receiverId)
            .OrderBy(message => message.TimeStampUtc);

        return await PagedList<UserMessage>.ToPagedListAsync(query, pageNumber, pageSize);
    }

    public async Task ReadAllAsync(Guid senderId, Guid receiverId)
    {
        await _dbContext.UserMessages
            .Where(message => message.Sender.Id == senderId 
                              && message.Receiver.Id == receiverId 
                              && !message.HasBeenDeleted)
            .UpdateAsync(message => new UserMessage()
            {
                HasBeenRead = true
            });
    }
}