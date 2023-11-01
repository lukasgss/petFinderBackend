using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Interfaces.Entities.Users;

public interface IUserRepository
{
    Task<SignInResult> CheckCredentials(User user, string password);
    Task<IdentityResult> RegisterUserAsync(User user, string password);
    Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
}