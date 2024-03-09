using Application.Common.Interfaces.General.Notifications;
using Application.Services.General.MessageNotifications;
using Application.Services.General.MessageNotifications.DTOs;
using NSubstitute;

namespace Tests.UnitTests;

public class NotificationServiceTests
{
	private readonly IMessageNotificationRepository _messageNotificationRepositoryMock;
	private readonly INotificationService _sut;

	private static readonly Guid UserId = Guid.NewGuid();

	public NotificationServiceTests()
	{
		_messageNotificationRepositoryMock = Substitute.For<IMessageNotificationRepository>();
		_sut = new MessageNotificationService(_messageNotificationRepositoryMock);
	}

	[Fact]
	public async Task Get_Amount_Of_Notifications_For_User_Returns_Amount()
	{
		const int notificationAmount = 55;
		_messageNotificationRepositoryMock.GetAmountOfNotificationsForUserAsync(UserId).Returns(notificationAmount);
		NotificationAmountResponse expectedNotificationAmount = new()
		{
			Amount = notificationAmount
		};

		NotificationAmountResponse notificationAmountResponse = await _sut.GetAmountOfNotificationsForUserAsync(UserId);

		Assert.Equivalent(expectedNotificationAmount, notificationAmountResponse);
	}
}