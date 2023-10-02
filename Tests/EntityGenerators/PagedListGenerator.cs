using System.Collections.Generic;
using Application.Common.Pagination;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class PagedListGenerator
{
    public static PagedList<UserMessage> GeneratePagedUserMessages()
    {
        List<UserMessage> userMessages = UserMessageGenerator.GenerateListOfUserMessages();
        return new PagedList<UserMessage>(userMessages, userMessages.Count, 1, 50);
    }
}