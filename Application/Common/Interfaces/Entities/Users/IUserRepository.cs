using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Entities.Users;

public interface IUserRepository
{
    Task<SignInResult> CheckCredentials(string email, string password);
    Task<IdentityResult> RegisterUserAsync(User user, string password);
    Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
}