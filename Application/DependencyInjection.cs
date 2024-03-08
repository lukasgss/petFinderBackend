using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Authorization;
using Application.Common.Interfaces.Converters;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.General.Notifications;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.Providers;
using Application.Common.Providers;
using Application.Marker;
using Application.Services.Authentication;
using Application.Services.Authorization;
using Application.Services.Converters;
using Application.Services.Entities;
using Application.Services.Entities.Comments;
using Application.Services.General.Images;
using Application.Services.General.Notifications;
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
		services.AddScoped<ISpeciesService, SpeciesService>();
		services.AddScoped<IAdoptionAlertService, AdoptionAlertService>();
		services.AddScoped<IUserMessageService, UserMessageService>();
		services.AddScoped<IIdConverterService, IdConverterService>();
		services.AddScoped<IImageProcessingService, ImageProcessingService>();
		services.AddScoped<INotificationService, NotificationService>();
		services.AddScoped<IMissingAlertCommentService, MissingAlertCommentService>();
		services.AddScoped<IAdoptionAlertCommentService, AdoptionAlertCommentService>();
		services.AddScoped<IFoundAnimalAlertService, FoundAnimalAlertService>();
		services.AddScoped<IVaccineService, VaccineService>();
		services.AddScoped<IUserImageSubmissionService, UserImageSubmissionService>();
		services.AddScoped<IPetImageSubmissionService, PetImageSubmissionService>();
		services.AddScoped<IFoundAlertImageSubmissionService, FoundAlertImageSubmissionService>();
		services.AddScoped<IFoundAnimalUserPreferencesService, FoundAnimalUserPreferencesService>();

		services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

		services.AddSingleton<IValueProvider, ValueProvider>();

		services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));
		services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

		services.Configure<MessagingSettings>(configuration.GetSection("MessagingSettings"));

		return services;
	}
}