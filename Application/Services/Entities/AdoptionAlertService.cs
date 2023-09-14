using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
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

    public async Task<AdoptionAlertResponse> CreateAsync(CreateAdoptionAlertRequest createAlertRequest,
        Guid userId)
    {
        Pet pet = await ValidateAndAssignPetAsync(createAlertRequest.PetId);
        ValidateIfUserIsOwnerOfPetForCreation(pet.Owner.Id, userId);

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

    private void ValidateIfUserIsOwnerOfPetForCreation(Guid actualPetOwnerId, Guid userId)
    {
        if (actualPetOwnerId != userId)
        {
            throw new UnauthorizedException("Não é possível cadastrar adoções para animais em que não é dono.");
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