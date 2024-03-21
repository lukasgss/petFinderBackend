using Application.Common.Interfaces.RealTimeCommunication;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.RealTimeCommunication;

public class RealTimeChatClient : IRealTimeChatClient
{
	private readonly IHubContext<ChatHub> _hubContext;

	public RealTimeChatClient(IHubContext<ChatHub> hubContext)
	{
		_hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
	}

	public async Task EditMessage(Guid senderId, Guid receiverId, EditedMessage message)
	{
		await _hubContext.Clients.Users(senderId.ToString(), receiverId.ToString())
			.SendAsync("EditMessage", message);
	}

	public async Task DeleteMessage(Guid senderId, Guid receiverId, DeletedMessage deletedMessage)
	{
		await _hubContext.Clients.Users(senderId.ToString(), receiverId.ToString())
			.SendAsync("DeleteMessage", deletedMessage);
	}
}