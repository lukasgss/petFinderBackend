using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Common.Providers;
using Application.Marker;
using Application.Services.Authentication;
using Application.Services.Authorization;
using Application.Services.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
        services.AddScoped<IBreedService, BreedService>();
        services.AddScoped<IMissingAlertService, MissingAlertService>();

        services.AddScoped<IGuidProvider, GuidProvider>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        
        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>(_ =>
        {
            IConfigurationSection jwtConfig = configuration.GetSection("JwtSettings");

            return new JwtTokenGenerator(
                secretKey: jwtConfig["SecretKey"],
                audience: jwtConfig["Audience"],
                issuer: jwtConfig["Issuer"],
                expiryInMinutes: int.Parse(jwtConfig["ExpiryInMinutes"])
            );
        });
        
        return services;
    }
}