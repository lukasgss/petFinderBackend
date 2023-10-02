using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;

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
            TotalCount = userMessageResponses.Count,
            TotalPages = 1
        };
    }
}