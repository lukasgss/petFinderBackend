using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserRepository(
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager) : base(dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<SignInResult> CheckCredentials(string email, string password)
    {
        return await _signInManager.PasswordSignInAsync(
            userName: email,
            password: password,
            isPersistent: false,
            lockoutOnFailure: true);
    }

    public async Task<IdentityResult> RegisterUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled)
    {
        return await _userManager.SetLockoutEnabledAsync(user, enabled);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email);
    }
}