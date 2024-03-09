namespace Application.Services.General.MessageNotifications;

public interface IMessageNotificationRepository
{
	Task<int> GetAmountOfNotificationsForUserAsync(Guid userId);
}