using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Entities;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UserService(
        IUserRepository userRepository,
        IGuidProvider guidProvider,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _guidProvider = guidProvider;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<UserDataResponse> GetUserByIdAsync(Guid userId)
    {
        User? searchedUser = await _userRepository.GetUserByIdAsync(userId);

        if (searchedUser is null)
        {
            throw new NotFoundException("Não foi possível obter o usuário com o id especificado.");
        }

        return searchedUser.ConvertToUserDataResponse();
    }

    public async Task<UserResponse> RegisterAsync(CreateUserRequest createUserRequest)
    {
        User userToCreate = new()
        {
            Id = _guidProvider.NewGuid(),
            FullName = createUserRequest.FullName,
            UserName = createUserRequest.Email,
            PhoneNumber = createUserRequest.PhoneNumber,
            Email = createUserRequest.Email,
            EmailConfirmed = true,
        };

        User? userAlreadyExists = await _userRepository.GetUserByEmailAsync(createUserRequest.Email);
        if (userAlreadyExists is not null)
        {
            throw new ConflictException("Usuário com o e-mail especificado já existe.");
        }

        IdentityResult registrationResult =
            await _userRepository.RegisterUserAsync(userToCreate, createUserRequest.Password);
        
        IdentityResult lockoutResult = await _userRepository.SetLockoutEnabledAsync(userToCreate, false);
        
        if (!registrationResult.Succeeded || !lockoutResult.Succeeded)
        {
            throw new InternalServerErrorException();
        }

        string jwtToken = _jwtTokenGenerator.GenerateToken(userToCreate.Id, userToCreate.FullName);

        return userToCreate.ConvertToUserResponse(jwtToken);
    }

    public async Task<UserResponse> LoginAsync(LoginUserRequest loginUserRequest)
    {
        User? userToLogin = await _userRepository.GetUserByEmailAsync(loginUserRequest.Email);
        SignInResult signInResult =
            await _userRepository.CheckCredentials(loginUserRequest.Email, loginUserRequest.Password);

        if (!signInResult.Succeeded || userToLogin is null)
        {
            if (signInResult.IsLockedOut)
            {
                throw new LockedException("Essa conta está bloqueada, aguarde e tente novamente.");
            }
            
            throw new UnauthorizedException("Credenciais inválidas.");
        }

        string jwtToken = _jwtTokenGenerator.GenerateToken(userToLogin.Id, userToLogin.FullName);

        return userToLogin.ConvertToUserResponse(jwtToken);
    }
}