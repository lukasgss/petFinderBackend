using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Pagination;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("/user-messages")]
public class UserMessageController : ControllerBase
{
    private readonly IUserMessageService _userMessageService;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public UserMessageController(IUserMessageService userMessageService, IUserAuthorizationService userAuthorizationService)
    {
        _userMessageService = userMessageService ?? throw new ArgumentNullException(nameof(userMessageService));
        _userAuthorizationService = userAuthorizationService ?? throw new ArgumentNullException(nameof(userAuthorizationService));
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedEntity<UserMessageResponse>>> GetAllMessages(
        [Required] Guid senderId, [Required] Guid receiverId, int pageNumber = 1, int pageSize = PagedList<UserMessage>.MaxPageSize)
    {
        Guid currentUserId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _userMessageService.GetMessagesAsync(
            senderId, receiverId, currentUserId, pageNumber, pageSize);
    }

    [HttpPost("send")]
    public async Task<ActionResult<UserMessageResponse>> SendAsync(SendUserMessageRequest messageRequest)
    {
        Guid senderId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        UserMessageResponse message = await _userMessageService.SendAsync(messageRequest, senderId);

        return new ObjectResult(message)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpPut("{messageId:long}")]
    public async Task<ActionResult<UserMessageResponse>> EditAsync(
        long messageId, EditUserMessageRequest editUserMessageRequest)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _userMessageService.EditAsync(messageId, editUserMessageRequest, userId, messageId);
    }

    [HttpDelete("{messageId:long}")]
    public async Task<ActionResult> DeleteAsync(long messageId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        await _userMessageService.DeleteAsync(messageId, userId);
        return Ok();
    }
}