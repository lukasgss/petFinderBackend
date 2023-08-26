using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Providers;
using Application.Common.Providers;
using Application.Services.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IGuidProvider, GuidProvider>();
        
        return services;
    }
}