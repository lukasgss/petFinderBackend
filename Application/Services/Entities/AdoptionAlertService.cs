using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Services.Entities;

public class AdoptionAlertService : IAdoptionAlertService
{
	private readonly IAdoptionAlertRepository _adoptionAlertRepository;
	private readonly IPetRepository _petRepository;
	private readonly IUserRepository _userRepository;
	private readonly IDateTimeProvider _dateTimeProvider;
	private readonly IGuidProvider _guidProvider;

	public AdoptionAlertService(
		IAdoptionAlertRepository adoptionAlertRepository,
		IPetRepository petRepository,
		IUserRepository userRepository,
		IDateTimeProvider dateTimeProvider,
		IGuidProvider guidProvider)
	{
		_adoptionAlertRepository =
			adoptionAlertRepository ?? throw new ArgumentNullException(nameof(adoptionAlertRepository));
		_petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
		_guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
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

		AdoptionAlert adoptionAlertToCreate = new()
		{
			Id = _guidProvider.NewGuid(),
			OnlyForScreenedProperties = createAlertRequest.OnlyForScreenedProperties,
			LocationLatitude = createAlertRequest.LocationLatitude,
			LocationLongitude = createAlertRequest.LocationLongitude,
			Description = createAlertRequest.Description,
			RegistrationDate = _dateTimeProvider.UtcNow(),
			AdoptionDate = null,
			Pet = pet,
			User = alertOwner,
		};

		_adoptionAlertRepository.Add(adoptionAlertToCreate);
		await _adoptionAlertRepository.CommitAsync();

		return adoptionAlertToCreate.ToAdoptionAlertResponse();
	}

	public async Task<AdoptionAlertResponse> EditAsync(EditAdoptionAlertRequest editAlertRequest, Guid userId,
		Guid routeId)
	{
		if (routeId != editAlertRequest.Id)
		{
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		AdoptionAlert adoptionAlertDb = await ValidateAndAssignAdoptionAlertAsync(editAlertRequest.Id);
		ValidateIfUserIsOwnerOfAlert(adoptionAlertDb.User.Id, userId);

		Pet pet = await ValidateAndAssignPetAsync(editAlertRequest.PetId);
		ValidateIfUserIsOwnerOfPet(pet.Owner.Id, userId);

		adoptionAlertDb.OnlyForScreenedProperties = editAlertRequest.OnlyForScreenedProperties;
		adoptionAlertDb.LocationLatitude = editAlertRequest.LocationLatitude;
		adoptionAlertDb.LocationLongitude = editAlertRequest.LocationLongitude;
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
			throw new ForbiddenException("Não é possível alterar o status de alertas em que não é dono.");
		}

		if (adoptionAlert.AdoptionDate is null)
		{
			adoptionAlert.AdoptionDate = _dateTimeProvider.DateOnlyNow();
		}
		else
		{
			adoptionAlert.AdoptionDate = null;
		}

		await _adoptionAlertRepository.CommitAsync();

		return adoptionAlert.ToAdoptionAlertResponse();
	}

	private static void ValidateIfUserIsOwnerOfAlert(Guid actualOwnerId, Guid userId)
	{
		if (actualOwnerId != userId)
		{
			throw new UnauthorizedException("Não é possível alterar alertas de adoção de outros usuários.");
		}
	}

	private static void ValidateIfUserIsOwnerOfPet(Guid actualPetOwnerId, Guid userId)
	{
		if (actualPetOwnerId != userId)
		{
			throw new UnauthorizedException(
				"Não é possível cadastrar ou editar adoções para animais em que não é dono.");
		}
	}

	private async Task<User> ValidateAndAssignUserAsync(Guid userId)
	{
		User? user = await _userRepository.GetUserByIdAsync(userId);
		if (user is null)
		{
			throw new NotFoundException("Usuário com o id especificado não existe.");
		}

		return user;
	}

	private async Task<Pet> ValidateAndAssignPetAsync(Guid petId)
	{
		Pet? pet = await _petRepository.GetPetByIdAsync(petId);
		if (pet is null)
		{
			throw new NotFoundException("Animal com o id especificado não existe.");
		}

		return pet;
	}

	private async Task<AdoptionAlert> ValidateAndAssignAdoptionAlertAsync(Guid alertId)
	{
		AdoptionAlert? adoptionAlert = await _adoptionAlertRepository.GetByIdAsync(alertId);
		if (adoptionAlert is null)
		{
			throw new NotFoundException("Alerta de adoção com o id especificado não existe.");
		}

		return adoptionAlert;
	}
}