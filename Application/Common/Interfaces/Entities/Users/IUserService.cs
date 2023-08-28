using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Users;

public interface IUserService
{
    Task<UserDataResponse> GetUserByIdAsync(Guid userId);
    Task<UserResponse> RegisterAsync(CreateUserRequest createUserRequest);
    Task<UserResponse> LoginAsync(LoginUserRequest loginUserRequest);
}