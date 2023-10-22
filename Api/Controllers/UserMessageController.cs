using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Pagination;
using Application.Common.Validations.Errors;
using Application.Common.Validations.UserMessages;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValidationResult = FluentValidation.Results.ValidationResult;

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
        SendUserMessageValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(messageRequest);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e =>
                new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }
        
        Guid senderId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        UserMessageResponse message = await _userMessageService.SendAsync(messageRequest, senderId);

        return new ObjectResult(message)
        {
            StatusCode = StatusCodes.Status201Created
        };
    }

    [HttpPut("{messageId:long}")]
    public async Task<ActionResult<UserMessageResponse>> EditAsync(
        long messageId, EditUserMessageRequest userMessage)
    {
        EditUserMessageValidator requestValidator = new();
        ValidationResult validationResult = requestValidator.Validate(userMessage);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e =>
                new ValidationError(e.PropertyName, e.ErrorMessage));
            return BadRequest(errors);
        }
        
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        return await _userMessageService.EditAsync(messageId, userMessage, userId, messageId);
    }

    [HttpDelete("{messageId:long}")]
    public async Task<ActionResult> DeleteAsync(long messageId)
    {
        Guid userId = _userAuthorizationService.GetUserIdFromJwtToken(User);

        await _userMessageService.DeleteAsync(messageId, userId);
        return Ok();
    }
}