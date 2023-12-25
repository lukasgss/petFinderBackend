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
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = Constants.UserMessageData.HasBeenEdited,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender,
            SenderId = Constants.UserMessageData.SenderId,
            Receiver = Constants.UserMessageData.Receiver,
            ReceiverId = Constants.UserMessageData.ReceiverId
        };
    }

    public static UserMessage GenerateEditedUserMessage()
    {
        return new UserMessage()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = true,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
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

    public static EditUserMessageRequest GenerateEditUserMessageRequest()
    {
        return new EditUserMessageRequest()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content
        };
    }

    public static List<UserMessageResponse> GenerateListOfUserMessageResponses()
    {
        List<UserMessageResponse> userMessageResponses = new(3);
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
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = Constants.UserMessageData.HasBeenEdited,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender.ToUserDataResponse(),
            Receiver = Constants.UserMessageData.Receiver.ToUserDataResponse(),
        };
    }

    public static UserMessageResponse GenerateEditedUserMessageResponse()
    {
        return new UserMessageResponse()
        {
            Id = Constants.UserMessageData.Id,
            Content = Constants.UserMessageData.Content,
            TimeStampUtc = Constants.UserMessageData.TimeStamp,
            HasBeenRead = Constants.UserMessageData.HasBeenRead,
            HasBeenEdited = true,
            HasBeenDeleted = Constants.UserMessageData.HasBeenDeleted,
            Sender = Constants.UserMessageData.Sender.ToUserDataResponse(),
            Receiver = Constants.UserMessageData.Receiver.ToUserDataResponse(),
        };
    }
}