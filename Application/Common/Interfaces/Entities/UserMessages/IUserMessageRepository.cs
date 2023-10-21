using Application.Common.Interfaces.GenericRepository;
using Application.Common.Pagination;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.UserMessages;

public interface IUserMessageRepository : IGenericRepository<UserMessage>
{
    Task<UserMessage?> GetByIdAsync(long messageId, Guid userId);
    Task<PagedList<UserMessage>> GetAllFromUserAsync(Guid senderId, Guid receiverId, int pageNumber, int pageSize);
    Task ReadAllAsync(Guid senderId, Guid receiverId);
}