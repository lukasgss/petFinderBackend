using System.Collections.Generic;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Domain.Entities;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators;

public static class UserMessageGenerator
{
    public static UserMessage GenerateUserMessage()
    {
        return new UserMessage()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStamp = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            Sender = Constants.UserMessageData.Sender,
            SenderId = Constants.UserMessageData.SenderId,
            Receiver = Constants.UserMessageData.Receiver,
            ReceiverId = Constants.UserMessageData.ReceiverId
        };
    }

    public static List<UserMessage> GenerateListOfUserMessages()
    {
        List<UserMessage> userMessages = new();
        for (int i = 0; i < 3; i++)
        {
            userMessages.Add(GenerateUserMessage());
        }

        return userMessages;
    }

    public static SendUserMessageRequest GenerateSendUserMessageRequest()
    {
        return new SendUserMessageRequest()
        {
            Content = Constants.UserMessageData.Content,
            ReceiverId = Constants.UserMessageData.ReceiverId
        };
    }
        
    public static List<UserMessageResponse> GenerateListOfUserMessageResponses()
    {
        List<UserMessageResponse> userMessageResponses = new();
        for (int i = 0; i < 3; i++)
        {
            userMessageResponses.Add(GenerateUserMessageResponse());
        }

        return userMessageResponses;
    }

    public static UserMessageResponse GenerateUserMessageResponse()
    {
        return new UserMessageResponse()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStamp = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            Sender = Constants.UserMessageData.Sender.ToUserDataResponse(),
            Receiver = Constants.UserMessageData.Receiver.ToUserDataResponse(),
        };
    }
}