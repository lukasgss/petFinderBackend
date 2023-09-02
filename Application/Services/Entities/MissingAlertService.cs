using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
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

    public async Task<MissingAlertResponse> GetMissingAlertByIdAsync(Guid missingAlertId)
    {
        MissingAlert? missingAlert = await _missingAlertRepository.GetMissingAlertByIdAsync(missingAlertId);
        if (missingAlert is null)
        {
            throw new NotFoundException("Alerta de desaparecimento com o id especificado não existe.");
        }

        return new MissingAlertResponse()
        {
            Id = missingAlert.Id,
            OwnerName = missingAlert.OwnerName,
            OwnerPhoneNumber = missingAlert.OwnerPhoneNumber,
            RegistrationDate = missingAlert.RegistrationDate,
            LastSeenLocationLatitude = missingAlert.LastSeenLocationLatitude,
            LastSeenLocationLongitude = missingAlert.LastSeenLocationLongitude,
            PetHasBeenRecovered = missingAlert.PetHasBeenRecovered,
            Pet = missingAlert.Pet.ConvertToPetResponseNoOwner(
                missingAlert.Pet.Colors.ConvertToListOfColorResponse(),
                missingAlert.Pet.Breed.ConvertToBreedResponse()),
            Owner = missingAlert.User?.ConvertToOwnerResponse()
        };
    }

    public async Task<MissingAlertResponse> CreateMissingAlertAsync(CreateMissingAlertRequest createMissingAlertRequest,
        Guid? userId)
    {
        Pet? missingPet = await _petRepository.GetPetByIdAsync(createMissingAlertRequest.PetId);
        if (missingPet is null)
        {
            throw new NotFoundException("Animal com o id especificado não existe.");
        }

        CheckUserPermission(userId, createMissingAlertRequest.UserId);

        User? petOwner = null;
        if (userId is not null)
        {
            petOwner = await _userRepository.GetUserByIdAsync((Guid)userId);
        }

        MissingAlert missingAlertToCreate = new()
        {
            Id = _guidProvider.NewGuid(),
            OwnerName = createMissingAlertRequest.OwnerName,
            OwnerPhoneNumber = createMissingAlertRequest.OwnerPhoneNumber,
            RegistrationDate = _dateTimeProvider.UtcNow(),
            LastSeenLocationLatitude = createMissingAlertRequest.LastSeenLocationLatitude,
            LastSeenLocationLongitude = createMissingAlertRequest.LastSeenLocationLongitude,
            PetHasBeenRecovered = false,
            Pet = missingPet,
            User = petOwner,
        };

        _missingAlertRepository.Add(missingAlertToCreate);
        await _missingAlertRepository.CommitAsync();

        return missingAlertToCreate.ConvertToMissingAlertResponse();
    }

    private void CheckUserPermission(Guid? userId, Guid? requestUserId)
    {
        if (userId is not null && userId != requestUserId)
        {
            throw new ForbiddenException("Não é possível criar alertas para outros usuários.");
        }
    }
}