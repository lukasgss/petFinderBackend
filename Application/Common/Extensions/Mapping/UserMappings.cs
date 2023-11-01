using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class UserMappings
{
    public static OwnerResponse ToOwnerResponse(this User user)
    {
        return new OwnerResponse()
        {
            FullName = user.FullName,
            Image = user.Image,
            PhoneNumber = user.PhoneNumber,
            Email = user.Email
        };
    }

    public static UserDataResponse ToUserDataResponse(this User user)
    {
        return new UserDataResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            Image = user.Image,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
        };
    }

    public static UserResponse ToUserResponse(this User user, string jwtToken)
    {
        return new UserResponse()
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Image = user.Image,
            PhoneNumber = user.PhoneNumber,
            Token = jwtToken
        };
    }
}