using Application.Common.Interfaces.Entities.User.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMappings
{
    public static OwnerResponse ConvertToOwnerResponse(this User user)
    {
        return new OwnerResponse()
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber
        };
    }
}