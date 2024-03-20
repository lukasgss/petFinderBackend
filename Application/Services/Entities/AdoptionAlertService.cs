using Application.Common.Calculators;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.General.Messages;
using Domain.Entities;
using Domain.Entities.Alerts;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Application.Services.Entities;

public class AdoptionAlertService : IAdoptionAlertService
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IPetRepository _petRepository;
	private readonly IUserRepository _userRepository;
	private readonly IAlertsMessagingService _alertsMessagingService;
	private readonly IValueProvider _valueProvider;
	private readonly ILogger<AdoptionAlertService> _logger;

	public AdoptionAlertService(
		IAdoptionAlertRepository adoptionAlertRepository,
		IPetRepository petRepository,
		IUserRepository userRepository,
		IAlertsMessagingService alertsMessagingService,
		IValueProvider valueProvider,
		ILogger<AdoptionAlertService> logger)
	{
		_adoptionAlertRepository =
			adoptionAlertRepository ?? throw new ArgumentNullException(nameof(adoptionAlertRepository));
		_petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_alertsMessagingService =
			alertsMessagingService ?? throw new ArgumentNullException(nameof(alertsMessagingService));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<AdoptionAlertResponse> GetByIdAsync(Guid alertId)
	{
		AdoptionAlert adoptionAlert = await ValidateAndAssignAdoptionAlertAsync(alertId);

		return adoptionAlert.ToAdoptionAlertResponse();
	}

	public async Task<PaginatedEntity<AdoptionAlertResponse>> ListAdoptionAlerts(
		AdoptionAlertFilters filters, int page = 1, int pageSize = 30)
	{
		if (page < 1 || pageSize < 1)
		{
			throw new BadRequestException("Insira um número e tamanho de página maior ou igual a 1.");
		}

		var filteredAlerts =
			await _adoptionAlertRepository.ListAdoptionAlerts(filters, page, pageSize);

		return filteredAlerts.ToAdoptionAlertResponsePagedList();
	}

	public async Task<AdoptionAlertResponse> CreateAsync(CreateAdoptionAlertRequest createAlertRequest,
		Guid userId)
	{
		Pet pet = await ValidateAndAssignPetAsync(createAlertRequest.PetId);
		ValidateIfUserIsOwnerOfPet(pet.Owner.Id, userId);

		User alertOwner = await ValidateAndAssignUserAsync(userId);

		Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
			createAlertRequest.LocationLatitude,
			createAlertRequest.LocationLongitude);

		AdoptionAlert alertToBeCreated = new()
		{
			Id = _valueProvider.NewGuid(),
			OnlyForScreenedProperties = createAlertRequest.OnlyForScreenedProperties,
			Location = location,
			Description = createAlertRequest.Description,
			RegistrationDate = _valueProvider.UtcNow(),
			AdoptionDate = null,
			Pet = pet,
			User = alertOwner,
		};

		_adoptionAlertRepository.Add(alertToBeCreated);
		await _adoptionAlertRepository.CommitAsync();

		_alertsMessagingService.PublishAdoptionAlert(alertToBeCreated);

		return alertToBeCreated.ToAdoptionAlertResponse();
	}

	public async Task<AdoptionAlertResponse> EditAsync(EditAdoptionAlertRequest editAlertRequest, Guid userId,
		Guid routeId)
	{
		if (routeId != editAlertRequest.Id)
		{
			_logger.LogInformation("Id {RouteId} não coincide com {AdoptionRequestId}", routeId, editAlertRequest.Id);
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		AdoptionAlert adoptionAlertDb = await ValidateAndAssignAdoptionAlertAsync(editAlertRequest.Id);
		ValidateIfUserIsOwnerOfAlert(adoptionAlertDb.User.Id, userId);

		Pet pet = await ValidateAndAssignPetAsync(editAlertRequest.PetId);
		ValidateIfUserIsOwnerOfPet(pet.Owner.Id, userId);

		Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
			editAlertRequest.LocationLatitude,
			editAlertRequest.LocationLongitude);

		adoptionAlertDb.OnlyForScreenedProperties = editAlertRequest.OnlyForScreenedProperties;
		adoptionAlertDb.Location = location;
		adoptionAlertDb.Description = editAlertRequest.Description;
		adoptionAlertDb.Pet = adoptionAlertDb.Pet;

		await _adoptionAlertRepository.CommitAsync();

		return adoptionAlertDb.ToAdoptionAlertResponse();
	}

	public async Task DeleteAsync(Guid alertId, Guid userId)
	{
		AdoptionAlert adoptionAlertDb = await ValidateAndAssignAdoptionAlertAsync(alertId);
		ValidateIfUserIsOwnerOfAlert(adoptionAlertDb.User.Id, userId);

		_adoptionAlertRepository.Delete(adoptionAlertDb);
		await _adoptionAlertRepository.CommitAsync();
	}

	public async Task<AdoptionAlertResponse> ToggleAdoptionAsync(Guid alertId, Guid userId)
	{
		AdoptionAlert adoptionAlert = await ValidateAndAssignAdoptionAlertAsync(alertId);

		if (userId != adoptionAlert.User.Id)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para alterar status de adoção em que o dono é {ActualAlertOwnerId}",
				userId, adoptionAlert.User.Id);
			throw new ForbiddenException("Não é possível alterar o status de alertas em que não é dono.");
		}

		if (adoptionAlert.AdoptionDate is null)
		{
			adoptionAlert.AdoptionDate = _valueProvider.DateOnlyNow();
		}
		else
		{
			adoptionAlert.AdoptionDate = null;
		}

		await _adoptionAlertRepository.CommitAsync();

		return adoptionAlert.ToAdoptionAlertResponse();
	}

	private void ValidateIfUserIsOwnerOfAlert(Guid actualOwnerId, Guid userId)
	{
		if (actualOwnerId != userId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para alterar adoção em que o dono é {ActualAlertOwnerId}",
				userId, actualOwnerId);
			throw new UnauthorizedException("Não é possível alterar alertas de adoção de outros usuários.");
		}
	}

	private void ValidateIfUserIsOwnerOfPet(Guid actualPetOwnerId, Guid userId)
	{
		if (actualPetOwnerId != userId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para cadastrar ou editar adoção em que o dono é {ActualPetOwnerId}",
				userId, actualPetOwnerId);
			throw new UnauthorizedException(
				"Não é possível cadastrar ou editar adoções para animais em que não é dono.");
		}
	}

	private async Task<User> ValidateAndAssignUserAsync(Guid userId)
	{
		User? user = await _userRepository.GetUserByIdAsync(userId);
		if (user is null)
		{
			_logger.LogInformation("Usuário {UserId} não existe", userId);
			throw new NotFoundException("Usuário com o id especificado não existe.");
		}

		return user;
	}

	private async Task<Pet> ValidateAndAssignPetAsync(Guid petId)
	{
		Pet? pet = await _petRepository.GetPetByIdAsync(petId);
		if (pet is null)
		{
			_logger.LogInformation("Pet {PetId} não existe", petId);
			throw new NotFoundException("Animal com o id especificado não existe.");
		}

		return pet;
	}

	private async Task<AdoptionAlert> ValidateAndAssignAdoptionAlertAsync(Guid alertId)
	{
		AdoptionAlert? adoptionAlert = await _adoptionAlertRepository.GetByIdAsync(alertId);
		if (adoptionAlert is null)
		{
			_logger.LogInformation("Alerta de adoção {AlertId} não existe", alertId);
			throw new NotFoundException("Alerta de adoção com o id especificado não existe.");
		}

		return adoptionAlert;
	}
}