using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMappings
{
    public static OwnerResponse ConvertToOwnerResponse(this User user)
    {
        return new OwnerResponse()
        {
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };
    }

    public static UserDataResponse ConvertToUserDataResponse(this User user)
    {
        return new UserDataResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
        };
    }

    public static UserResponse ConvertToUserResponse(this User user, string jwtToken)
    {
        return new UserResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Token = jwtToken
        };
    }
}