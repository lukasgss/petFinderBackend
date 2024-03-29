﻿using Application.Common.Calculators;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Providers;
using Application.Extensions;
using Application.Services.General.Messages;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Application.Services.Entities;

public class FoundAnimalAlertService : IFoundAnimalAlertService
{
	private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly IUserRepository _userRepository;
	private readonly IColorRepository _colorRepository;
	private readonly IFoundAlertImageSubmissionService _imageSubmissionService;
	private readonly IAlertsMessagingService _alertsMessagingService;
	private readonly IValueProvider _valueProvider;
	private readonly ILogger<FoundAnimalAlertService> _logger;

	public FoundAnimalAlertService(
		IFoundAnimalAlertRepository foundAnimalAlertRepository,
		ISpeciesRepository speciesRepository,
		IBreedRepository breedRepository,
		IUserRepository userRepository,
		IColorRepository colorRepository,
		IFoundAlertImageSubmissionService imageSubmissionService,
		IAlertsMessagingService alertsMessagingService,
		IValueProvider valueProvider,
		ILogger<FoundAnimalAlertService> logger)
	{
		_foundAnimalAlertRepository = foundAnimalAlertRepository ??
		                              throw new ArgumentNullException(nameof(foundAnimalAlertRepository));
		_imageSubmissionService = imageSubmissionService ??
		                          throw new ArgumentNullException(nameof(imageSubmissionService));
		_colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
		_speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
		_breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_alertsMessagingService =
			alertsMessagingService ?? throw new ArgumentNullException(nameof(alertsMessagingService));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<FoundAnimalAlertResponse> GetByIdAsync(Guid alertId)
	{
		FoundAnimalAlert? foundAnimalAlert = await _foundAnimalAlertRepository.GetByIdAsync(alertId);
		if (foundAnimalAlert is null)
		{
			_logger.LogInformation("Alerta {FoundAlertId} não existe", alertId);
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		return foundAnimalAlert.ToFoundAnimalAlertResponse();
	}

	public async Task<PaginatedEntity<FoundAnimalAlertResponse>> ListFoundAnimalAlerts(
		FoundAnimalAlertFilters filters, int page, int pageSize)
	{
		if (page < 1 || pageSize < 1)
		{
			throw new BadRequestException("Insira um número e tamanho de página maior ou igual a 1.");
		}

		filters.Name = filters.Name.ToStrWithoutDiacritics();

		var filteredAlerts = await _foundAnimalAlertRepository.ListAlertsAsync(filters, page, pageSize);

		return filteredAlerts.ToFoundAnimalAlertResponsePagedList();
	}

	public async Task<FoundAnimalAlertResponse> CreateAsync(CreateFoundAnimalAlertRequest createAlertRequest,
		Guid userId)
	{
		Species? species = await _speciesRepository.GetSpeciesByIdAsync(createAlertRequest.SpeciesId);
		if (species is null)
		{
			_logger.LogInformation("Espécie {SpeciesId} não existe", createAlertRequest.SpeciesId);
			throw new NotFoundException("Espécie com o id especificado não existe.");
		}

		List<Color> colors = await ValidateAndAssignColorsAsync(createAlertRequest.ColorIds);
		Breed? breed = await ValidateAndQueryBreed(createAlertRequest.BreedId);

		User? userCreating = await _userRepository.GetUserByIdAsync(userId);

		Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(createAlertRequest.FoundLocationLatitude,
			createAlertRequest.FoundLocationLongitude);

		FoundAnimalAlert alertToBeCreated = new()
		{
			Id = _valueProvider.NewGuid(),
			Name = createAlertRequest.Name,
			Description = createAlertRequest.Description,
			Location = location,
			Size = createAlertRequest.Size,
			RegistrationDate = _valueProvider.UtcNow(),
			RecoveryDate = null,
			Gender = createAlertRequest.Gender,
			Age = createAlertRequest.Age,
			Colors = colors,
			Species = species,
			Breed = breed,
			User = userCreating!,
			Images = new List<FoundAnimalAlertImage>(0),
		};

		var uploadedImageUrls = await UploadAlertImages(alertToBeCreated, createAlertRequest.Images);
		alertToBeCreated.Images = uploadedImageUrls;

		_foundAnimalAlertRepository.Add(alertToBeCreated);
		await _foundAnimalAlertRepository.CommitAsync();

		_alertsMessagingService.PublishFoundAlert(alertToBeCreated);

		return alertToBeCreated.ToFoundAnimalAlertResponse();
	}

	public async Task<FoundAnimalAlertResponse> EditAsync(EditFoundAnimalAlertRequest editAlertRequest, Guid userId,
		Guid routeId)
	{
		if (routeId != editAlertRequest.Id)
		{
			_logger.LogInformation("Id {RouteId} não coincide com {FoundAlertId}", routeId, editAlertRequest.Id);
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		FoundAnimalAlert? alertToBeEdited = await _foundAnimalAlertRepository.GetByIdAsync(editAlertRequest.Id);
		if (alertToBeEdited is null)
		{
			_logger.LogInformation("Alerta {FoundAlertId} não existe", editAlertRequest.Id);
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		bool canUpdate = ValidatePermissionToChange(alertToBeEdited, userId);
		if (!canUpdate)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para editar alerta de animal encontrado de {ActualOwnerId}",
				userId, alertToBeEdited.User.Id);
			throw new ForbiddenException("Não é possível editar alertas de outros usuários.");
		}

		Species? species = await _speciesRepository.GetSpeciesByIdAsync(editAlertRequest.SpeciesId);
		if (species is null)
		{
			_logger.LogInformation("Espécie {SpeciesId} não existe", editAlertRequest.SpeciesId);
			throw new NotFoundException("Espécie com o id especificado não existe.");
		}

		List<Color> colors = await ValidateAndAssignColorsAsync(editAlertRequest.ColorIds);
		Breed? breed = await ValidateAndQueryBreed(editAlertRequest.BreedId);

		var uploadedImageUrls = await UpdateAlertImages(alertToBeEdited, editAlertRequest.Images);

		Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(editAlertRequest.FoundLocationLatitude,
			editAlertRequest.FoundLocationLongitude);

		alertToBeEdited.Name = editAlertRequest.Name;
		alertToBeEdited.Description = editAlertRequest.Description;
		alertToBeEdited.Location = location;
		alertToBeEdited.Images = uploadedImageUrls;
		alertToBeEdited.Size = editAlertRequest.Size;
		alertToBeEdited.Species = species;
		alertToBeEdited.Breed = breed;
		alertToBeEdited.Gender = editAlertRequest.Gender;
		alertToBeEdited.Age = editAlertRequest.Age;
		alertToBeEdited.Colors = colors;

		await _foundAnimalAlertRepository.CommitAsync();

		return alertToBeEdited.ToFoundAnimalAlertResponse();
	}

	public async Task DeleteAsync(Guid alertId, Guid userId)
	{
		FoundAnimalAlert? alertToDelete = await _foundAnimalAlertRepository.GetByIdAsync(alertId);
		if (alertToDelete is null)
		{
			_logger.LogInformation("Alerta {FoundAlertId} não existe", alertId);
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		bool canUpdate = ValidatePermissionToChange(alertToDelete, userId);
		if (!canUpdate)
		{
			_logger.LogInformation("User {UserId} não possui permissão para excluir alerta {FoundAlertId}",
				userId, alertId);
			throw new ForbiddenException("Não é possível excluir alertas de outros usuários.");
		}

		_foundAnimalAlertRepository.Delete(alertToDelete);
		await _foundAnimalAlertRepository.CommitAsync();
	}

	public async Task<FoundAnimalAlertResponse> ToggleAlertStatus(Guid alertId, Guid userId)
	{
		FoundAnimalAlert? alertToToggle = await _foundAnimalAlertRepository.GetByIdAsync(alertId);
		if (alertToToggle is null)
		{
			_logger.LogInformation("Alerta {FoundAlertId} não existe", alertId);
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		if (alertToToggle.User.Id != userId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para alterar status de {FoundAlertId}", userId, alertId);
			throw new ForbiddenException("Não é possível alterar o status de alertas de outros usuários.");
		}

		alertToToggle.RecoveryDate = alertToToggle.RecoveryDate is null ? _valueProvider.DateOnlyNow() : null;

		await _foundAnimalAlertRepository.CommitAsync();

		return alertToToggle.ToFoundAnimalAlertResponse();
	}

	private static bool ValidatePermissionToChange(FoundAnimalAlert alertToBeEdited, Guid userId)
	{
		return userId == alertToBeEdited.User.Id;
	}

	private async Task<Breed?> ValidateAndQueryBreed(int? breedId)
	{
		if (breedId is null)
		{
			return null;
		}

		Breed? breed = await _breedRepository.GetBreedByIdAsync((int)breedId);
		if (breed is null)
		{
			_logger.LogInformation("Raça {BreedId} não existe", breedId);
			throw new NotFoundException("Raça com o id especificado não existe.");
		}

		return breed;
	}

	private async Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds)
	{
		List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
		if (colors.Count != colorIds.Count || colors.Count == 0)
		{
			_logger.LogInformation("Alguma das cores {@ColorIds} não existe", colors);
			throw new NotFoundException("Alguma das cores especificadas não existe.");
		}

		return colors;
	}

	private async Task<List<FoundAnimalAlertImage>> UploadAlertImages(FoundAnimalAlert animalAlert,
		List<IFormFile> submittedImages)
	{
		var uploadedImageUrls =
			await _imageSubmissionService.UploadImagesAsync(animalAlert.Id, submittedImages);

		return uploadedImageUrls
			.Select(image => new FoundAnimalAlertImage()
				{ ImageUrl = image, FoundAnimalAlertId = animalAlert.Id, FoundAnimalAlert = animalAlert })
			.ToList();
	}

	private async Task<List<FoundAnimalAlertImage>> UpdateAlertImages(
		FoundAnimalAlert animalAlert, List<IFormFile> submittedImages)
	{
		var uploadedImageUrls =
			await _imageSubmissionService.UpdateImagesAsync(animalAlert.Id, submittedImages, animalAlert.Images.Count);

		return uploadedImageUrls
			.Select(image => new FoundAnimalAlertImage()
				{ ImageUrl = image, FoundAnimalAlertId = animalAlert.Id, FoundAnimalAlert = animalAlert })
			.ToList();
	}
}