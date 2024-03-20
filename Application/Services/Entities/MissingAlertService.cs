using Application.Common.Calculators;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Application.Services.Entities;

public class MissingAlertService : IMissingAlertService
{
	private readonly IMissingAlertRepository _missingAlertRepository;
	private readonly IPetRepository _petRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;
	private readonly ILogger<MissingAlertService> _logger;

	public MissingAlertService(IMissingAlertRepository missingAlertRepository,
		IPetRepository petRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider,
		ILogger<MissingAlertService> logger)
	{
		_missingAlertRepository =
			missingAlertRepository ?? throw new ArgumentNullException(nameof(missingAlertRepository));
		_petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<MissingAlertResponse> GetByIdAsync(Guid missingAlertId)
	{
		MissingAlert missingAlert = await ValidateAndAssignMissingAlertAsync(missingAlertId);

		return missingAlert.ToMissingAlertResponse();
	}

	public async Task<PaginatedEntity<MissingAlertResponse>> ListMissingAlerts(
		MissingAlertFilters filters, int page, int pageSize)
	{
		if (page < 1 || pageSize < 1)
		{
			throw new BadRequestException("Insira um número e tamanho de página maior ou igual a 1.");
		}

		var filteredAlerts = await _missingAlertRepository.ListMissingAlerts(filters, page, pageSize);

		return filteredAlerts.ToMissingAlertResponsePagedList();
	}

	public async Task<MissingAlertResponse> CreateAsync(CreateMissingAlertRequest createAlertRequest,
		Guid userId)
	{
		Pet missingPet = await ValidateAndAssignPetAsync(createAlertRequest.PetId);

		CheckUserPermissionToCreate(missingPet.Owner.Id, userId);

		User petOwner = await ValidateAndAssignUserAsync(userId);

		Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
			createAlertRequest.LastSeenLocationLatitude,
			createAlertRequest.LastSeenLocationLongitude);

		MissingAlert missingAlertToCreate = new()
		{
			Id = _valueProvider.NewGuid(),
			RegistrationDate = _valueProvider.UtcNow(),
			Location = location,
			Description = createAlertRequest.Description,
			RecoveryDate = null,
			Pet = missingPet,
			User = petOwner,
		};

		_missingAlertRepository.Add(missingAlertToCreate);
		await _missingAlertRepository.CommitAsync();

		return missingAlertToCreate.ToMissingAlertResponse();
	}

	public async Task<MissingAlertResponse> EditAsync(
		EditMissingAlertRequest editAlertRequest, Guid userId, Guid routeId)
	{
		if (editAlertRequest.Id != routeId)
		{
			_logger.LogInformation("Id {RouteId} não coincide com {AdoptionRequestId}", routeId, editAlertRequest.Id);
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		MissingAlert dbMissingAlert = await ValidateAndAssignMissingAlertAsync(editAlertRequest.Id);
		Pet pet = await ValidateAndAssignPetAsync(editAlertRequest.PetId);

		CheckUserPermissionToEdit(dbMissingAlert.User.Id, userId);

		User user = await ValidateAndAssignUserAsync(userId);

		Point location = CoordinatesCalculator.CreatePointBasedOnCoordinates(
			editAlertRequest.LastSeenLocationLatitude,
			editAlertRequest.LastSeenLocationLongitude);

		dbMissingAlert.Location = location;
		dbMissingAlert.Description = editAlertRequest.Description;
		dbMissingAlert.Pet = pet;
		dbMissingAlert.User = user;

		await _missingAlertRepository.CommitAsync();

		return dbMissingAlert.ToMissingAlertResponse();
	}

	public async Task DeleteAsync(Guid missingAlertId, Guid userId)
	{
		MissingAlert alertToDelete = await ValidateAndAssignMissingAlertAsync(missingAlertId);

		if (alertToDelete.User.Id != userId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para excluir alerta {MissingAlertId}", userId, missingAlertId);
			throw new ForbiddenException("Não é possível excluir alertas de outros usuários.");
		}

		_missingAlertRepository.Delete(alertToDelete);
		await _missingAlertRepository.CommitAsync();
	}

	public async Task<MissingAlertResponse> ToggleFoundStatusAsync(Guid alertId, Guid userId)
	{
		MissingAlert missingAlert = await ValidateAndAssignMissingAlertAsync(alertId);

		if (userId != missingAlert.User.Id)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para alterar status do alerta {MissingAlertId}",
				userId, alertId);
			throw new ForbiddenException("Não é possível marcar alertas de outros usuários como encontrado.");
		}

		if (missingAlert.RecoveryDate is null)
		{
			missingAlert.RecoveryDate = _valueProvider.DateOnlyNow();
		}
		else
		{
			missingAlert.RecoveryDate = null;
		}

		await _missingAlertRepository.CommitAsync();

		return missingAlert.ToMissingAlertResponse();
	}

	private void CheckUserPermissionToCreate(Guid userId, Guid requestUserId)
	{
		if (userId != requestUserId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para criar alerta para usuário {RequestUserId}",
				userId, requestUserId);
			throw new ForbiddenException("Não é possível criar alertas para outros usuários.");
		}
	}

	private void CheckUserPermissionToEdit(Guid? userId, Guid requestUserId)
	{
		if (userId != requestUserId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para editar alerta do usuário {ActualOwnerId}",
				userId, requestUserId);
			throw new ForbiddenException("Não é possível editar alertas de outros usuários.");
		}
	}

	private async Task<MissingAlert> ValidateAndAssignMissingAlertAsync(Guid missingAlertId)
	{
		MissingAlert? dbMissingAlert = await _missingAlertRepository.GetByIdAsync(missingAlertId);
		if (dbMissingAlert is null)
		{
			_logger.LogInformation("Alerta {MissingAlertId} não existe", missingAlertId);
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		return dbMissingAlert;
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
}