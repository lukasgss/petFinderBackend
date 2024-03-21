namespace Application.Common.Interfaces.RealTimeCommunication;

public interface IRealTimeChatClient
{
	Task EditMessage(Guid senderId, Guid receiverId, EditedMessage message);
	Task DeleteMessage(Guid senderId, Guid receiverId, DeletedMessage deletedMessage);
}