using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMessageMappings
{
    public static UserMessageResponse ToUserMessageResponse(this UserMessage userMessage)
    {
        return new UserMessageResponse()
        {
            Id = userMessage.Id,
            Content = userMessage.Content,
            TimeStampUtc = userMessage.TimeStampUtc,
            HasBeenRead = userMessage.HasBeenRead,
            HasBeenEdited = userMessage.HasBeenEdited,
            HasBeenDeleted = userMessage.HasBeenDeleted,
            Sender = userMessage.Sender.ToUserDataResponse(),
            Receiver = userMessage.Receiver.ToUserDataResponse()
        };
    }
}