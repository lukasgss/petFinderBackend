using System.Text;
using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Authorization.Facebook;
using Application.Common.Interfaces.Authorization.Google;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Api.Extensions;

public static class AuthSetup
{
	public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddIdentity<User, IdentityRole<Guid>>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequiredLength = 6;
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedEmail = false;
			}).AddEntityFrameworkStores<AppDbContext>()
			.AddDefaultTokenProviders();

		services.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				IConfigurationSection configJwt = configuration.GetSection(JwtConfig.SectionName);
				string secret = configJwt["SecretKey"]!;

				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = configJwt["Issuer"],
					ValidAudience = configJwt["Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(secret)
					)
				};
			});

		services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));
		services.Configure<GoogleAuthConfig>(configuration.GetSection("Authentication:Google"));
		services.Configure<FacebookAuthConfig>(configuration.GetSection("Authentication:Facebook"));
	}
}