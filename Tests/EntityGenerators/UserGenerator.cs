using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.EntityGenerators;

public static class UserGenerator
{
    public static User GenerateUser()
    {
        return new User()
        {
            Id = Constants.UserData.Id,
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            Image = Constants.UserData.ImageUrl,
            UserName = Constants.UserData.UserName,
            Email = Constants.UserData.Email,
            EmailConfirmed = Constants.UserData.EmailConfirmed
        };
    }

    public static User GenerateUserWithRandomId()
    {
        return new User()
        {
            Id = Guid.NewGuid(),
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            UserName = Constants.UserData.UserName,
            Image = Constants.UserData.ImageUrl,
            Email = Constants.UserData.Email,
            EmailConfirmed = Constants.UserData.EmailConfirmed
        };
    }

    public static LoginUserRequest GenerateLoginUserRequest()
    {
        return new LoginUserRequest()
        {
            Email = Constants.UserData.Email,
            Password = Constants.UserData.Password,
        };
    }

    public static CreateUserRequest GenerateCreateUserRequest()
    {
        return new CreateUserRequest()
        {
            Email = Constants.UserData.Email,
            FullName = Constants.UserData.FullName,
            PhoneNumber = Constants.UserData.PhoneNumber,
            Image = Constants.UserData.ImageFile,
            Password = Constants.UserData.Password,
            ConfirmPassword = Constants.UserData.Password,
        };
    }
}