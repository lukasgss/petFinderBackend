using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class UserGenerator
{
    public static User GenerateUser()
    {
        return new User()
        {
            Id = Guid.NewGuid(),
            FullName = "fullName",
            PhoneNumber = "(11) 11111-1111",
            UserName = "email@email.com",
            Email = "email@email.com",
            EmailConfirmed = true
        };
    }
}