using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Pagination;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class PagedListMappings
{ 
        public static PaginatedEntity<UserMessageResponse> ToUserMessageResponsePagedList(this PagedList<UserMessage> pagedUserMessages)
        {
                List<UserMessageResponse> userMessageResponses = pagedUserMessages
                        .Select(message => message.ToUserMessageResponse())
                        .ToList();

                return new PaginatedEntity<UserMessageResponse>()
                {
                        Data = userMessageResponses,
                        CurrentPage = pagedUserMessages.CurrentPage,
                        TotalCount = pagedUserMessages.TotalCount,
                        TotalPages = pagedUserMessages.TotalPages
                };
        }
}