using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.ExternalServices.AWS;
using Application.Common.Interfaces.Messaging;
using Application.Services.General.Notifications;
using Infrastructure.DependencyInjections;
using Infrastructure.ExternalServices.AWS;
using Infrastructure.ExternalServices.Configs;
using Infrastructure.Messaging;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IPetRepository, PetRepository>();
		services.AddScoped<IBreedRepository, BreedRepository>();
		services.AddScoped<IColorRepository, ColorRepository>();
		services.AddScoped<ISpeciesRepository, SpeciesRepository>();
		services.AddScoped<IMissingAlertRepository, MissingAlertRepository>();
		services.AddScoped<ISpeciesRepository, SpeciesRepository>();
		services.AddScoped<IAdoptionAlertRepository, AdoptionAlertRepository>();
		services.AddScoped<IUserMessageRepository, UserMessageRepository>();
		services.AddScoped<IMessagingService, MessagingService>();
		services.AddScoped<INotificationRepository, NotificationRepository>();
		services.AddScoped<IMissingAlertCommentRepository, MissingAlertCommentRepository>();
		services.AddScoped<IAdoptionAlertCommentRepository, AdoptionAlertCommentRepository>();
		services.AddScoped<IFoundAnimalAlertRepository, FoundAnimalAlertRepository>();
		services.AddScoped<IVaccineRepository, VaccineRepository>();

		services.AddScoped<IAwsS3Client, AwsS3Client>();
		services.Configure<AwsData>(configuration.GetSection("AWS"));
		services.ConfigureAws(configuration);

		services.AddDbContext<AppDbContext>(options =>
			options.UseNpgsql(configuration.GetConnectionString("DefaultConnection") ?? string.Empty)
				.UseEnumCheckConstraints());

		return services;
	}
}