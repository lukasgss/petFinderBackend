using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Users;

public interface IUserService
{
	Task<UserDataResponse> GetUserByIdAsync(Guid userId);
	Task<UserResponse> RegisterAsync(CreateUserRequest createUserRequest);
	Task<UserDataResponse> EditAsync(EditUserRequest editUserRequest, Guid userId, Guid routeId);
	Task<UserResponse> LoginAsync(LoginUserRequest loginUserRequest);
	Task<UserResponse> ExternalLoginAsync(ExternalAuthRequest externalAuth);
	Task ConfirmEmailAsync(string hashedUserId, string token);
	Task<TokensResponse> RefreshToken(Guid userId);
}