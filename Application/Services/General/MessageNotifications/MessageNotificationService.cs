using Application.Common.Interfaces.General.Notifications;
using Application.Services.General.MessageNotifications.DTOs;

namespace Application.Services.General.MessageNotifications;

public class MessageNotificationService : INotificationService
{
	private readonly IMessageNotificationRepository _messageNotificationRepository;

	public MessageNotificationService(IMessageNotificationRepository messageNotificationRepository)
	{
		_messageNotificationRepository =
			messageNotificationRepository ?? throw new ArgumentNullException(nameof(messageNotificationRepository));
	}

	public async Task<NotificationAmountResponse> GetAmountOfNotificationsForUserAsync(Guid userId)
	{
		int amountOfNotifications = await _messageNotificationRepository.GetAmountOfNotificationsForUserAsync(userId);

		return new NotificationAmountResponse()
		{
			Amount = amountOfNotifications
		};
	}
}