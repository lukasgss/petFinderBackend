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
            TimeStamp = userMessage.TimeStamp,
            HasBeenRead = userMessage.HasBeenRead,
            Sender = userMessage.Sender.ToUserDataResponse(),
            Receiver = userMessage.Receiver.ToUserDataResponse()
        };
    }
}