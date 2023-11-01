using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
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
    private readonly IIdConverterService _idConverterServiceMock;
    private readonly IUserService _sut;

    private static readonly User User = UserGenerator.GenerateUser();
    private static readonly UserDataResponse UserDataResponse = User.ToUserDataResponse();
    private static readonly UserResponse UserResponse = User.ToUserResponse(Constants.UserData.JwtToken);
    private static readonly CreateUserRequest CreateUserRequest = UserGenerator.GenerateCreateUserRequest();
    private static readonly LoginUserRequest LoginUserRequest = UserGenerator.GenerateLoginUserRequest();

    public UserServiceTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _guidProviderMock = Substitute.For<IGuidProvider>();
        _jwtTokenGeneratorMock = Substitute.For<IJwtTokenGenerator>();
        IHttpContextAccessor httpRequestMock = Substitute.For<IHttpContextAccessor>();
        IMessagingService messagingServiceMock = Substitute.For<IMessagingService>();
        LinkGenerator linkGeneratorMock = Substitute.For<LinkGenerator>();
        _idConverterServiceMock = Substitute.For<IIdConverterService>();

        _sut = new UserService(
            _userRepositoryMock,
            _guidProviderMock,
            _jwtTokenGeneratorMock,
            httpRequestMock,
            messagingServiceMock,
            linkGeneratorMock,
            _idConverterServiceMock);
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
        _userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);

        UserDataResponse userResponse = await _sut.GetUserByIdAsync(User.Id);

        Assert.Equivalent(UserDataResponse, userResponse);
    }

    [Fact]
    public async Task Register_Attempt_With_Already_Existing_Email_Throws_ConflictException()
    {
        _guidProviderMock.NewGuid().Returns(User.Id);
        _userRepositoryMock.GetUserByEmailAsync(CreateUserRequest.Email).Returns(User);

        async Task Result() => await _sut.RegisterAsync(CreateUserRequest);

        var exception = await Assert.ThrowsAsync<ConflictException>(Result);
        Assert.Equal("Usuário com o e-mail especificado já existe.", exception.Message);
    }

    [Fact]
    public async Task Register_Attempt_With_Any_Registration_Error_Throws_InternalServerErrorException()
    {
        _guidProviderMock.NewGuid().Returns(Constants.UserData.Id);
        _userRepositoryMock.GetUserByEmailAsync(Constants.UserData.Email).ReturnsNull();
        IdentityResult expectedIdentityResult = new FakeIdentityResult(succeeded: false);
        _userRepositoryMock.RegisterUserAsync(Arg.Any<User>(), CreateUserRequest.Password)
            .Returns(expectedIdentityResult);
        _userRepositoryMock.SetLockoutEnabledAsync(Arg.Any<User>(), false).Returns(expectedIdentityResult);

        async Task Result() => await _sut.RegisterAsync(CreateUserRequest);

        await Assert.ThrowsAsync<InternalServerErrorException>(Result);
    }

    [Fact]
    public async Task Registration_Returns_User_Response()
    {
        // Arrange
        _guidProviderMock.NewGuid().Returns(Constants.UserData.Id);
        _userRepositoryMock.GetUserByEmailAsync(Constants.UserData.Email).ReturnsNull();

        IdentityResult expectedIdentityResult = new FakeIdentityResult(succeeded: true);
        _userRepositoryMock.RegisterUserAsync(Arg.Any<User>(), CreateUserRequest.Password)
            .Returns(expectedIdentityResult);
        _userRepositoryMock.SetLockoutEnabledAsync(Arg.Any<User>(), false)
            .Returns(expectedIdentityResult);
        _jwtTokenGeneratorMock.GenerateToken(User.Id, Constants.UserData.FullName)
            .Returns(Constants.UserData.JwtToken);

        // Act
        UserResponse userResponse = await _sut.RegisterAsync(CreateUserRequest);

        // Assert
        Assert.Equivalent(UserResponse, userResponse);
    }

    [Fact]
    public async Task Login_With_Locked_Account_Throws_LockedException()
    {
        _userRepositoryMock.GetUserByEmailAsync(LoginUserRequest.Email).Returns(User);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: true);
        _userRepositoryMock.CheckCredentials(User, LoginUserRequest.Password)
            .Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(LoginUserRequest);

        var exception = await Assert.ThrowsAsync<LockedException>(Result);
        Assert.Equal("Essa conta está bloqueada, aguarde e tente novamente.", exception.Message);
    }

    [Fact]
    public async Task Login_With_Invalid_Credentials_Throws_UnauthorizedException()
    {
        _userRepositoryMock.GetUserByEmailAsync(LoginUserRequest.Email).Returns(User);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: false, isLockedOut: false);
        _userRepositoryMock.CheckCredentials(User, LoginUserRequest.Password)
            .Returns(fakeSignInResult);

        async Task Result() => await _sut.LoginAsync(LoginUserRequest);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Credenciais inválidas.", exception.Message);
    }

    [Fact]
    public async Task Login_With_Valid_Credentials_Returns_UserResponse()
    {
        _userRepositoryMock.GetUserByEmailAsync(LoginUserRequest.Email).Returns(User);
        FakeSignInResult fakeSignInResult = new FakeSignInResult(succeeded: true, isLockedOut: false);
        _userRepositoryMock.CheckCredentials(Arg.Any<User>(), LoginUserRequest.Password)
            .Returns(fakeSignInResult);
        _jwtTokenGeneratorMock.GenerateToken(User.Id, User.FullName).Returns(Constants.UserData.JwtToken);

        UserResponse userResponse = await _sut.LoginAsync(LoginUserRequest);

        Assert.Equivalent(UserResponse, userResponse);
    }

    [Fact]
    public async Task Confirm_Email_With_Invalid_User_Id_Throws_BadRequestException()
    {
        Guid decodedUserId = Guid.NewGuid();
        _idConverterServiceMock.DecodeShortIdToGuid(Arg.Any<string>()).Returns(decodedUserId);
        _userRepositoryMock.GetUserByIdAsync(decodedUserId).ReturnsNull();

        async Task Result() => await _sut.ConfirmEmailAsync("hashedUserId", "token");

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível ativar o email com os dados informados.", exception.Message);
    }

    [Fact]
    public async Task Confirm_Email_With_Confirmation_Fail_Throws_BadRequestException()
    {
        Guid decodedUserId = Guid.NewGuid();
        _idConverterServiceMock.DecodeShortIdToGuid(Arg.Any<string>()).Returns(decodedUserId);
        _userRepositoryMock.GetUserByIdAsync(decodedUserId).Returns(User);
        FakeIdentityResult fakeIdentityResult = new(succeeded: false);
        _userRepositoryMock.ConfirmEmailAsync(User, "token").Returns(fakeIdentityResult);

        async Task Result() => await _sut.ConfirmEmailAsync("hashedUserId", "token");

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Não foi possível ativar o email com os dados informados.", exception.Message);
    }
}