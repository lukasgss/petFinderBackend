using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Tests.EntityGenerators.Alerts;

namespace Tests.EntityGenerators;

public static class PaginatedEntityGenerator
{
    public static PaginatedEntity<UserMessageResponse> GeneratePaginatedUserMessageResponse()
    {
        var userMessageResponses = UserMessageGenerator.GenerateListOfUserMessageResponses();
        return new PaginatedEntity<UserMessageResponse>()
        {
            Data = userMessageResponses,
            CurrentPage = 1,
            CurrentPageCount = userMessageResponses.Count,
            TotalPages = 1
        };
    }

    public static PaginatedEntity<AdoptionAlertResponse> GeneratePaginatedAdoptionAlertResponse()
    {
        var adoptionAlertResponses = AdoptionAlertGenerator.GenerateListOfAlertsResponse();
        return new PaginatedEntity<AdoptionAlertResponse>()
        {
            Data = adoptionAlertResponses,
            CurrentPage = 1,
            CurrentPageCount = adoptionAlertResponses.Count,
            TotalPages = 1
        };
    }
}