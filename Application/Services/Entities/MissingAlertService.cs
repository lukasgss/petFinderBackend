using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts;
using Application.Common.Interfaces.Entities.Alerts.DTOs;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Services.Entities;

public class MissingAlertService : IMissingAlertService
{
    private readonly IMissingAlertRepository _missingAlertRepository;
    private readonly IPetRepository _petRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MissingAlertService(IMissingAlertRepository missingAlertRepository,
        IPetRepository petRepository,
        IUserRepository userRepository,
        IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _missingAlertRepository =
            missingAlertRepository ?? throw new ArgumentNullException(nameof(missingAlertRepository));
        _petRepository = petRepository ?? throw new ArgumentNullException(nameof(petRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<MissingAlertResponse> GetByIdAsync(Guid missingAlertId)
    {
        MissingAlert missingAlert = await ValidateAndAssignMissingAlertAsync(missingAlertId);

        return missingAlert.ToMissingAlertResponse();
    }

    public async Task<MissingAlertResponse> CreateAsync(CreateMissingAlertRequest createMissingAlertRequest,
        Guid userId)
    {
        Pet missingPet = await ValidateAndAssignPetAsync(createMissingAlertRequest.PetId);

        CheckUserPermissionToCreate(missingPet.Owner.Id, userId);
        
        User petOwner = await ValidateAndAssignUserAsync(userId);

        MissingAlert missingAlertToCreate = new()
        {
            Id = _guidProvider.NewGuid(),
            OwnerName = petOwner.FullName,
            OwnerPhoneNumber = petOwner.PhoneNumber,
            RegistrationDate = _dateTimeProvider.UtcNow(),
            LastSeenLocationLatitude = createMissingAlertRequest.LastSeenLocationLatitude,
            LastSeenLocationLongitude = createMissingAlertRequest.LastSeenLocationLongitude,
            RecoveryDate = null,
            Pet = missingPet,
            User = petOwner,
        };

        _missingAlertRepository.Add(missingAlertToCreate);
        await _missingAlertRepository.CommitAsync();

        return missingAlertToCreate.ToMissingAlertResponse();
    }

    public async Task<MissingAlertResponse> EditAsync(EditMissingAlertRequest editMissingAlertRequest,
        Guid userId,
        Guid routeId)
    {
        if (editMissingAlertRequest.Id != routeId)
        {
            throw new BadRequestException("Id da rota não coincide com o id especificado.");
        }

        MissingAlert dbMissingAlert = await ValidateAndAssignMissingAlertAsync(editMissingAlertRequest.Id);
        Pet pet = await ValidateAndAssignPetAsync(editMissingAlertRequest.PetId);

        CheckUserPermissionToEdit(dbMissingAlert.User.Id, userId);

        User user = await ValidateAndAssignUserAsync(userId);

        dbMissingAlert.OwnerName = user.FullName;
        dbMissingAlert.OwnerPhoneNumber = user.PhoneNumber;
        dbMissingAlert.LastSeenLocationLatitude = editMissingAlertRequest.LastSeenLocationLatitude;
        dbMissingAlert.LastSeenLocationLongitude = editMissingAlertRequest.LastSeenLocationLongitude;
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
            throw new ForbiddenException("Não é possível excluir alertas de outros usuários.");
        }

        _missingAlertRepository.Delete(alertToDelete);
        await _missingAlertRepository.CommitAsync();
    }

    public async Task<MissingAlertResponse> MarkAsResolvedAsync(Guid alertId, Guid userId)
    {
        MissingAlert missingAlert = await ValidateAndAssignMissingAlertAsync(alertId);

        if (userId != missingAlert.User.Id)
        {
            throw new ForbiddenException("Não é possível marcar alertas de outros usuários como encontrado.");
        }

        missingAlert.RecoveryDate = _dateTimeProvider.DateOnlyNow();
        await _missingAlertRepository.CommitAsync();

        return missingAlert.ToMissingAlertResponse();
    }

    private static void CheckUserPermissionToCreate(Guid userId, Guid requestUserId)
    {
        if (userId != requestUserId)
        {
            throw new ForbiddenException("Não é possível criar alertas para outros usuários.");
        }
    }

    private static void CheckUserPermissionToEdit(Guid? userId, Guid requestUserId)
    {
        if (userId != requestUserId)
        {
            throw new ForbiddenException("Não é possível editar alertas de outros usuários.");
        }
    }
    
    private async Task<MissingAlert> ValidateAndAssignMissingAlertAsync(Guid missingAlertId)
    {
        MissingAlert? dbMissingAlert = await _missingAlertRepository.GetByIdAsync(missingAlertId);
        if (dbMissingAlert is null)
        {
            throw new NotFoundException("Alerta com o id especificado não existe.");
        }

        return dbMissingAlert;
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
}