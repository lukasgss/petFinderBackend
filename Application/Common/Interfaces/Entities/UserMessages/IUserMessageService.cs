using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;

namespace Application.Common.Interfaces.Entities.UserMessages;

public interface IUserMessageService
{
	Task<UserMessageResponse> GetByIdAsync(long messageId, Guid userId);

	Task<PaginatedEntity<UserMessageResponse>> GetMessagesAsync(
		Guid senderId, Guid receiverId, Guid currentUserId, int pageNumber, int pageSize);

	Task<UserMessageResponse> SendAsync(Guid? senderId, Guid? receiverId, string messageContent);

	Task<UserMessageResponse> EditAsync(
		long messageId, EditUserMessageRequest editRequest, Guid userId, long routeId);

	Task DeleteAsync(long messageId, Guid userId);
}