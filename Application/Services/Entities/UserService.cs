using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Authorization.Facebook;
using Application.Common.Interfaces.Authorization.Google;
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
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Application.Services.Entities;

public class UserService : IUserService
{
	private readonly IUserRepository _userRepository;
	private readonly ITokenGenerator _tokenGenerator;
	private readonly IHttpContextAccessor _httpRequest;
	private readonly IMessagingService _messagingService;
	private readonly LinkGenerator _linkGenerator;
	private readonly IIdConverterService _idConverterService;
	private readonly IUserImageSubmissionService _userImageSubmissionService;
	private readonly IExternalAuthHandler _externalAuthHandler;
	private readonly IValueProvider _valueProvider;
	private readonly ILogger<UserService> _logger;

	private const string GoogleProvider = "GOOGLE";
	private const string FacebookProvider = "FACEBOOK";

	public UserService(
		IUserRepository userRepository,
		ITokenGenerator tokenGenerator,
		IHttpContextAccessor httpRequest,
		IMessagingService messagingService,
		LinkGenerator linkGenerator,
		IIdConverterService idConverterService,
		IUserImageSubmissionService userImageSubmissionService,
		IExternalAuthHandler externalAuthHandler,
		IValueProvider valueProvider,
		ILogger<UserService> logger)
	{
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
		_httpRequest = httpRequest ?? throw new ArgumentNullException(nameof(httpRequest));
		_messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
		_linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
		_idConverterService = idConverterService ?? throw new ArgumentNullException(nameof(idConverterService));
		_userImageSubmissionService =
			userImageSubmissionService ?? throw new ArgumentNullException(nameof(userImageSubmissionService));
		_externalAuthHandler = externalAuthHandler ?? throw new ArgumentNullException(nameof(externalAuthHandler));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<UserDataResponse> GetUserByIdAsync(Guid userId)
	{
		User? searchedUser = await _userRepository.GetUserByIdAsync(userId);

		if (searchedUser is null)
		{
			_logger.LogInformation("Não foi possível obter usuário {UserId}", userId);
			throw new NotFoundException("Não foi possível obter o usuário com o id especificado.");
		}

		return searchedUser.ToUserDataResponse();
	}

	public async Task<UserResponse> RegisterAsync(CreateUserRequest createUserRequest)
	{
		Guid userId = _valueProvider.NewGuid();

		string userImageUrl = await _userImageSubmissionService.UploadUserImageAsync(userId, createUserRequest.Image);

		User userToCreate = new()
		{
			Id = userId,
			FullName = createUserRequest.FullName,
			UserName = createUserRequest.Email,
			PhoneNumber = createUserRequest.PhoneNumber,
			Image = userImageUrl,
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

		TokensResponse tokens = _tokenGenerator.GenerateTokens(userToCreate.Id, userToCreate.FullName);

		return userToCreate.ToUserResponse(tokens);
	}

	public async Task<UserDataResponse> EditAsync(EditUserRequest editUserRequest, Guid userId, Guid routeId)
	{
		if (editUserRequest.Id != routeId)
		{
			_logger.LogInformation("Id {RouteId} não coincide com {UserId}", routeId, userId);
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		if (editUserRequest.Id != userId)
		{
			_logger.LogInformation("Usuário {UserId} não possui permissão para editar {OtherUserId}",
				userId, editUserRequest.Id);
			throw new ForbiddenException("Você não possui permissão para editar este usuário.");
		}

		User? user = await _userRepository.GetUserByIdAsync(editUserRequest.Id);
		if (user is null)
		{
			_logger.LogInformation("Usuário {UserId} não existe", editUserRequest.Id);
			throw new NotFoundException("Usuário com o id especificado não existe.");
		}

		string userImageUrl = await _userImageSubmissionService.UploadUserImageAsync(userId, editUserRequest.Image);

		user.FullName = editUserRequest.FullName;
		// TODO: Later on, add some kind of phone validation with SMS
		user.PhoneNumber = editUserRequest.PhoneNumber;
		user.Image = userImageUrl;

		await _userRepository.CommitAsync();

		return user.ToUserDataResponse();
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

		TokensResponse tokens = _tokenGenerator.GenerateTokens(userToLogin.Id, userToLogin.FullName);

		return userToLogin.ToUserResponse(tokens);
	}


	public async Task<UserResponse> ExternalLoginAsync(ExternalAuthRequest externalAuth)
	{
		switch (externalAuth.Provider.ToUpperInvariant())
		{
			case GoogleProvider:
				return await GoogleLoginAsync(externalAuth);
			case FacebookProvider:
				return await FacebookLoginAsync(externalAuth);
			default:
				_logger.LogInformation("Provedor {Provider} não suportado", externalAuth.Provider);
				throw new BadRequestException("Provedor de autenticação externa não suportada.");
		}
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

	public async Task<TokensResponse> RefreshToken(Guid userId)
	{
		User? user = await _userRepository.GetUserByIdAsync(userId);
		if (user is null)
		{
			_logger.LogInformation("Usuário {UserId} não existe", userId);
			throw new NotFoundException("Não foi possível encontrar um usuário com esse id.");
		}

		if (await _userRepository.IsLockedOutAsync(user))
		{
			throw new LockedException("Essa conta está bloqueada.");
		}

		// TODO: Add user e-mail confirmation
		// if (!await _userRepository.IsEmailConfirmedAsync(user))
		// {
		// 	throw new UnauthorizedException("É necessário confirmar o e-mail antes de realizar login.");
		// }

		return _tokenGenerator.GenerateTokens(userId, user.FullName);
	}

	private async Task<User> RegisterUserFromExternalAuthProviderAsync(ExternalAuthPayload payload, UserLoginInfo info)
	{
		User? user = await _userRepository.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
		if (user is not null)
		{
			return user;
		}

		user = await _userRepository.FindByEmailAsync(payload.Email);
		if (user is null)
		{
			string userImageUrl = _userImageSubmissionService.ValidateUserImage(payload.Image);
			user = new User
			{
				Id = _valueProvider.NewGuid(),
				Email = payload.Email,
				UserName = payload.Email,
				FullName = payload.FullName,
				Image = userImageUrl,
				EmailConfirmed = true
			};
			await _userRepository.RegisterUserAsync(user);
			await _userRepository.AddLoginAsync(user, info);
		}
		else
		{
			await _userRepository.AddLoginAsync(user, info);
		}

		return user;
	}

	private async Task<UserResponse> GoogleLoginAsync(ExternalAuthRequest externalAuth)
	{
		GooglePayload? payload = await _externalAuthHandler.ValidateGoogleToken(externalAuth);
		if (payload is null)
		{
			throw new BadRequestException("Não foi possível realizar o login com o Google.");
		}

		ExternalAuthPayload externalAuthPayload = new()
		{
			Email = payload.Email,
			FullName = payload.FullName,
			Image = payload.Image
		};
		UserLoginInfo info = new(GoogleProvider, payload.Subject, GoogleProvider);

		User user = await RegisterUserFromExternalAuthProviderAsync(externalAuthPayload, info);

		TokensResponse tokens = _tokenGenerator.GenerateTokens(user.Id, user.FullName);

		return user.ToUserResponse(tokens);
	}

	private async Task<UserResponse> FacebookLoginAsync(ExternalAuthRequest externalAuth)
	{
		FacebookPayload? payload = await _externalAuthHandler.ValidateFacebookToken(externalAuth);
		if (payload is null)
		{
			throw new BadRequestException("Não foi possível realizar o login com o Facebook.");
		}

		ExternalAuthPayload externalAuthPayload = new()
		{
			Email = payload.Email,
			FullName = payload.FullName,
			Image = payload.Image
		};
		UserLoginInfo info = new(FacebookProvider, payload.UserId, FacebookProvider);

		User user = await RegisterUserFromExternalAuthProviderAsync(externalAuthPayload, info);

		TokensResponse tokens = _tokenGenerator.GenerateTokens(user.Id, user.FullName);

		return user.ToUserResponse(tokens);
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

		await _messagingService.SendAccountConfirmationMessageAsync(userToCreate.Email!, confirmationLink);
	}
}