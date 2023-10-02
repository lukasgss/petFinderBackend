using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/api/user-messages")]
public class UserMessageController : ControllerBase
{
    private readonly IUserMessageService _userMessageService;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public UserMessageController(IUserMessageService userMessageService, IUserAuthorizationService userAuthorizationService)
    {
        _userMessageService = userMessageService ?? throw new ArgumentNullException(nameof(userMessageService));
        _userAuthorizationService = userAuthorizationService ?? throw new ArgumentNullException(nameof(userAuthorizationService));
    }

    [Authorize]
    [HttpGet("{messageId:long}", Name = "GetUserMessageById")]
    public async Task<ActionResult<UserMessageResponse>> GetUserMessageById(long messageId)
    {
        Guid receiverId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _userMessageService.GetByIdAsync(messageId, receiverId);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PaginatedEntity<UserMessageResponse>>> GetAllMessages(Guid senderId, int pageNumber, int pageSize)
    {
        Guid receiverId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _userMessageService.GetMessagesFromSenderAsync(senderId, receiverId, pageNumber, pageSize);
    }

    [Authorize]
    [HttpPost("send")]
    public async Task<ActionResult<UserMessageResponse>> SendAsync(SendUserMessageRequest messageRequest)
    {
        Guid senderId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        UserMessageResponse message = await _userMessageService.SendAsync(messageRequest, senderId);
        return new CreatedAtRouteResult(nameof(GetUserMessageById), new { messageId = message.Id }, message);
    }
}