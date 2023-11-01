using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Application.Services.Entities;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpContextAccessor _httpRequest;
    private readonly IMessagingService _messagingService;
    private readonly LinkGenerator _linkGenerator;
    private readonly IIdConverterService _idConverterService;
    private readonly IImageSubmissionService _imageSubmissionService;

    public UserService(
        IUserRepository userRepository,
        IGuidProvider guidProvider,
        IJwtTokenGenerator jwtTokenGenerator,
        IHttpContextAccessor httpRequest,
        IMessagingService messagingService,
        LinkGenerator linkGenerator,
        IIdConverterService idConverterService,
        IImageSubmissionService imageSubmissionService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _httpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
        _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        _idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
        _imageSubmissionService =
            imageSubmissionService ?? throw new ArgumentNullException(nameof(imageSubmissionService));
    }

    public async Task<UserDataResponse> GetUserByIdAsync(Guid userId)
    {
        User? searchedUser = await _userRepository.GetUserByIdAsync(userId);

        if (searchedUser is null)
        {
            throw new NotFoundException("Não foi possível obter o usuário com o id especificado.");
        }

        return searchedUser.ToUserDataResponse();
    }

    public async Task<UserResponse> RegisterAsync(CreateUserRequest createUserRequest)
    {
        Guid userId = _guidProvider.NewGuid();

        string uploadedImageUrl = await _imageSubmissionService.UploadUserImageAsync(userId, createUserRequest.Image);

        User userToCreate = new()
        {
            Id = userId,
            FullName = createUserRequest.FullName,
            UserName = createUserRequest.Email,
            PhoneNumber = createUserRequest.PhoneNumber,
            Image = uploadedImageUrl,
            Email = createUserRequest.Email,
            EmailConfirmed = false
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

        await GenerateAccountConfirmationMessage(userToCreate);

        string jwtToken = _jwtTokenGenerator.GenerateToken(userToCreate.Id, userToCreate.FullName);

        return userToCreate.ToUserResponse(jwtToken);
    }

    public async Task<UserResponse> LoginAsync(LoginUserRequest loginUserRequest)
    {
        User? userToLogin = await _userRepository.GetUserByEmailAsync(loginUserRequest.Email);
        if (userToLogin is null)
        {
            // The userToLogin object is assigned to avoid time based attacks where
            // it's possible to enumerate valid emails based on response times from
            // the server
            userToLogin = new User()
            {
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        SignInResult signInResult = await _userRepository.CheckCredentials(userToLogin, loginUserRequest.Password);

        if (!signInResult.Succeeded || userToLogin is null)
        {
            if (signInResult.IsLockedOut)
            {
                throw new LockedException("Essa conta está bloqueada, aguarde e tente novamente.");
            }

            throw new UnauthorizedException("Credenciais inválidas.");
        }

        string jwtToken = _jwtTokenGenerator.GenerateToken(userToLogin.Id, userToLogin.FullName);

        return userToLogin.ToUserResponse(jwtToken);
    }

    public async Task ConfirmEmailAsync(string hashedUserId, string token)
    {
        Guid decodedUserId = _idConverterService.DecodeShortIdToGuid(hashedUserId);

        User? user = await _userRepository.GetUserByIdAsync(decodedUserId);
        if (user is null)
        {
            throw new BadRequestException("Não foi possível ativar o email com os dados informados.");
        }

        IdentityResult result = await _userRepository.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new BadRequestException("Não foi possível ativar o email com os dados informados.");
        }
    }

    private async Task GenerateAccountConfirmationMessage(User userToCreate)
    {
        string token = await _userRepository.GenerateEmailConfirmationTokenAsync(userToCreate);
        string hashedUserId = _idConverterService.ConvertGuidToShortId(userToCreate.Id);

        string confirmationLink = _linkGenerator.GetUriByAction(
            httpContext: _httpRequest.HttpContext!,
            action: "ConfirmEmail",
            controller: "User",
            values:
            new { userId = hashedUserId, token })!;

        await _messagingService.SendAccountConfirmationMessageAsync(userToCreate.Email, confirmationLink);
    }
}