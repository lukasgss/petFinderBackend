using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;

namespace Application.Common.Interfaces.Entities.UserMessages;

public interface IUserMessageService
{
    Task<UserMessageResponse> GetByIdAsync(long messageId, Guid receiverId);
    Task<PaginatedEntity<UserMessageResponse>> GetMessagesFromSenderAsync(
        Guid senderId,
        Guid receiverId,
        int pageNumber,
        int pageSize);
    Task<UserMessageResponse> SendAsync(SendUserMessageRequest messageRequest, Guid senderId);
}