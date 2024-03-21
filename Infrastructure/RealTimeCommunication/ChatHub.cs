using System.Security.Claims;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.RealTimeCommunication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.RealTimeCommunication;

public class ChatHub : Hub
{
	private readonly IUserMessageService _userMessageService;

	public ChatHub(IUserMessageService userMessageService)
	{
		_userMessageService = userMessageService ?? throw new ArgumentNullException(nameof(userMessageService));
	}

	[Authorize]
	public async Task SendMessage(string receiverId, string message)
	{
		string? userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		Guid? senderId = null;
		if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid senderResult))
		{
			senderId = senderResult;
		}

		Guid? nullableReceiverId = null;
		if (!string.IsNullOrEmpty(userId) && Guid.TryParse(receiverId, out Guid receiverResult))
		{
			nullableReceiverId = receiverResult;
		}

		var messageResponse = await _userMessageService.SendAsync(senderId, nullableReceiverId, message);
		SentMessage sentMessage = new()
		{
			Id = messageResponse.Id,
			SenderId = messageResponse.Sender.Id,
			ReceiverId = messageResponse.Receiver.Id,
			Content = messageResponse.Content
		};

		await Clients.Users(userId!, receiverId).SendAsync("ReceiveMessage", sentMessage);
	}
}