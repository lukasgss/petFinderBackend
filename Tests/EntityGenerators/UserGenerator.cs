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
            PhoneNumber = "(21) 98121-1828",
            UserName = "email@email.com",
            Email = "email@email.com",
            EmailConfirmed = true
        };
    }
}