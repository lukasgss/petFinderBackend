namespace Application.Services.General.Notifications;

public interface INotificationRepository
{
    Task<int> GetAmountOfNotificationsForUserAsync(Guid userId);
}