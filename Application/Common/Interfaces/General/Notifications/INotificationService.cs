using Application.Services.General.MessageNotifications.DTOs;

namespace Application.Common.Interfaces.General.Notifications;

public interface INotificationService
{
	Task<NotificationAmountResponse> GetAmountOfNotificationsForUserAsync(Guid userId);
}