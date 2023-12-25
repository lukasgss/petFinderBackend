using System.Collections.Generic;
using Application.Common.Pagination;
using Domain.Entities;
using Domain.Entities.Alerts;
using Tests.EntityGenerators.Alerts;

namespace Tests.EntityGenerators;

public static class PagedListGenerator
{
    public static PagedList<UserMessage> GeneratePagedUserMessages()
    {
        List<UserMessage> userMessages = UserMessageGenerator.GenerateListOfUserMessages();
        return new PagedList<UserMessage>(userMessages, userMessages.Count, pageNumber: 1, pageSize: 50);
    }

    public static PagedList<AdoptionAlert> GeneratePagedAdoptionAlerts()
    {
        List<AdoptionAlert> adoptionAlerts = AdoptionAlertGenerator.GenerateListOfAlerts();
        return new PagedList<AdoptionAlert>(adoptionAlerts, adoptionAlerts.Count, pageNumber: 1, pageSize: 50);
    }
}