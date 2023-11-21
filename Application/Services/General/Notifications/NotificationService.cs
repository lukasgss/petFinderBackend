using Application.Common.Interfaces.General.Notifications;
using Application.Services.General.Notifications.DTOs;

namespace Application.Services.General.Notifications;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository =
            notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
    }

    public async Task<NotificationAmountResponse> GetAmountOfNotificationsForUserAsync(Guid userId)
    {
        int amountOfNotifications = await _notificationRepository.GetAmountOfNotificationsForUserAsync(userId);

        return new NotificationAmountResponse()
        {
            Amount = amountOfNotifications
        };
    }
}