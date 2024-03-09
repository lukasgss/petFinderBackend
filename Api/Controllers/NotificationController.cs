using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.General.Notifications;
using Application.Services.General.MessageNotifications.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("/api/notifications")]
public class NotificationController : ControllerBase
{
	private readonly INotificationService _notificationService;
	private readonly IUserAuthorizationService _userAuthorizationService;

	public NotificationController(
		INotificationService notificationService,
		IUserAuthorizationService userAuthorizationService)
	{
		_notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
		_userAuthorizationService = userAuthorizationService ??
		                            throw new ArgumentNullException(nameof(userAuthorizationService));
	}

	[HttpGet("amount")]
	public async Task<NotificationAmountResponse> GetAmountForUser()
	{
		Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

		return await _notificationService.GetAmountOfNotificationsForUserAsync(userId);
	}
}