using Application.Common.Interfaces.General.Notifications;
using Application.Services.General.Notifications;
using Application.Services.General.Notifications.DTOs;
using NSubstitute;

namespace Tests.UnitTests;

public class NotificationServiceTests
{
    private readonly INotificationRepository _notificationRepositoryMock;
    private readonly INotificationService _sut;

    private static readonly Guid UserId = Guid.NewGuid();

    public NotificationServiceTests()
    {
        _notificationRepositoryMock = Substitute.For<INotificationRepository>();
        _sut = new NotificationService(_notificationRepositoryMock);
    }

    [Fact]
    public async Task Get_Amount_Of_Notifications_For_User_Returns_Amount()
    {
        const int notificationAmount = 55;
        _notificationRepositoryMock.GetAmountOfNotificationsForUserAsync(UserId).Returns(notificationAmount);
        NotificationAmountResponse expectedNotificationAmount = new()
        {
            Amount = notificationAmount
        };

        NotificationAmountResponse notificationAmountResponse = await _sut.GetAmountOfNotificationsForUserAsync(UserId);

        Assert.Equivalent(expectedNotificationAmount, notificationAmountResponse);
    }
}