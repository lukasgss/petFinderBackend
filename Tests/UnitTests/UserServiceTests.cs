using System;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.Fakes.Identity;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.UnitTests;

public class UserServiceTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IGuidProvider _guidProviderMock;
    private readonly IJwtTokenGenerator _jwtTokenGeneratorMock;
    private readonly IUserService _sut;

    public UserServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _guidProviderMock = Substitute.For<IGuidProvider>();
        _jwtTokenGeneratorMock = Substitute.For<IJwtTokenGenerator>();

        _sut = new UserService(_userRepositoryMock, _guidProviderMock, _jwtTokenGeneratorMock);
    }

    [Fact]
    public async Task Get_Non_Existent_User_By_Id_Throws_NotFoundException()
    {
        _userRepositoryMock.GetUserByIdAsync(Constants.UserData.Id).ReturnsNull();

        async Task Result() => await _sut.GetUserByIdAsync(Constants.UserData.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Não foi possível obter o usuário com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Get_User_By_Id_Returns_User_Data()
    {
        User searchedUser = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(searchedUser.Id).Returns(searchedUser);
        UserDataResponse expectedUser = searchedUser.ConvertToUserDataResponse();

        UserDataResponse userResponse = await _sut.GetUserByIdAsync(searchedUser.Id);

        Assert.Equivalent(expectedUser, userResponse);
    }

    [Fact]
    public async Task Register_Attempt_With_Already_Existing_Email_Throws_ConflictException()
    {
        CreateUserRequest createUserRequest = UserGenerator.GenerateCreateUserRequest();
        User alreadyExistingUser = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByEmailAsync(createUserRequest.Email).Returns(alreadyExistingUser);

        async Task Result() => await _sut.RegisterAsync(createUserRequest);

        var exception = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Usuário com o e-mail especificado já existe.", exception.Message);
    }

    [Fact]
    public async Task Register_Attempt_With_Any_Registration_Error_Throws_InternalServerErrorException()
    {
        _guidProviderMock.NewGuid().Returns(Constants.UserData.Id);
        _userRepositoryMock.GetUserByEmailAsync(Constants.UserData.Email).ReturnsNull();
        CreateUserRequest createUserRequest = UserGenerator.GenerateCreateUserRequest();
        IdentityResult expectedIdentityResult = new FakeIdentityResult(false);
        _userRepositoryMock.RegisterUserAsync(Arg.Any<User>(), createUserRequest.Password).Returns(expectedIdentityResult);
        _userRepositoryMock.SetLockoutEnabledAsync(Arg.Any<User>(), false).Returns(expectedIdentityResult);

        async Task Result() => await _sut.RegisterAsync(createUserRequest);

        await Assert.ThrowsAsync<InternalServerErrorException>(Result);
    }

    [Fact]
    public async Task Registration_Returns_User_Response()
    {
        // Arrange
        _guidProviderMock.NewGuid().Returns(Constants.UserData.Id);
        _userRepositoryMock.GetUserByEmailAsync(Constants.UserData.Email).ReturnsNull();
        CreateUserRequest createUserRequest = UserGenerator.GenerateCreateUserRequest();
        
        IdentityResult expectedIdentityResult = new FakeIdentityResult(true);
        _userRepositoryMock.RegisterUserAsync(Arg.Any<User>(), createUserRequest.Password)
            .Returns(expectedIdentityResult);
        _userRepositoryMock.SetLockoutEnabledAsync(Arg.Any<User>(), false)
            .Returns(expectedIdentityResult);
        _jwtTokenGeneratorMock.GenerateToken(Constants.UserData.Id, Constants.UserData.FullName).Returns(Constants.UserData.JwtToken);
        
        UserResponse expectedUserResponse = UserGenerator.GenerateUser().ConvertToUserResponse(Constants.UserData.JwtToken);

        // Act
        UserResponse userResponse = await _sut.RegisterAsync(createUserRequest);
        
        // Assert
        Assert.Equivalent(expectedUserResponse, userResponse);       
    }

    [Fact]
    public async Task Login_With_Locked_Account_Throws_LockedException()
    {
        LoginUserRequest loginUserRequest = UserGenerator.GenerateLoginUserRequest();
        User user = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByEmailAsync(loginUserRequest.Email).Returns(user);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: true);
        _userRepositoryMock.CheckCredentials(user, loginUserRequest.Password)
            .Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(loginUserRequest);

        var exception = await Assert.ThrowsAsync<LockedException>(Result);
        Assert.Equal("Essa conta está bloqueada, aguarde e tente novamente.", exception.Message);
    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Throws_UnauthorizedException()
    {
        LoginUserRequest loginUserRequest = UserGenerator.GenerateLoginUserRequest();
        User user = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByEmailAsync(loginUserRequest.Email).Returns(user);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: false);
        _userRepositoryMock.CheckCredentials(user, loginUserRequest.Password)
            .Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(loginUserRequest);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_UserResponse()
    {
        LoginUserRequest loginUserRequest = UserGenerator.GenerateLoginUserRequest();
        User user = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByEmailAsync(loginUserRequest.Email).Returns(user);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: true, isLockedOut: false);
        _userRepositoryMock.CheckCredentials(user, loginUserRequest.Password)
            .Returns(fakeSignInResult);
        _jwtTokenGeneratorMock.GenerateToken(user.Id, user.FullName).Returns(Constants.UserData.JwtToken);
        UserResponse expectedUserResponse = UserGenerator.GenerateUser().ConvertToUserResponse(Constants.UserData.JwtToken);

        UserResponse userResponse = await _sut.LoginAsync(loginUserRequest);

        Assert.Equivalent(expectedUserResponse, userResponse);
    }
}