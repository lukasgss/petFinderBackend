using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Providers;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Application.Services.Entities;

public class FoundAnimalAlertService : IFoundAnimalAlertService
{
    private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IBreedRepository _breedRepository;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IImageSubmissionService _imageSubmissionService;

    public FoundAnimalAlertService(
        IFoundAnimalAlertRepository foundAnimalAlertRepository,
        ISpeciesRepository speciesRepository,
        IBreedRepository breedRepository,
        IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider,
        IImageSubmissionService imageSubmissionService)
    {
        _foundAnimalAlertRepository = foundAnimalAlertRepository ??
                                      throw new ArgumentNullException(nameof(foundAnimalAlertRepository));
        _speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
        _breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _imageSubmissionService =
            imageSubmissionService ?? throw new ArgumentNullException(nameof(imageSubmissionService));
    }

    public async Task<FoundAnimalAlertResponse> GetByIdAsync(Guid alertId)
    {
        var foundAnimalAlert = await _foundAnimalAlertRepository.GetByIdAsync(alertId);
        if (foundAnimalAlert is null)
        {
            throw new NotFoundException("Alerta com o id especificado não existe.");
        }
        
        return foundAnimalAlert.ToFoundAnimalAlertResponse();
    }

    public async Task<FoundAnimalAlertResponse> CreateAsync(CreateFoundAnimalAlertRequest createAlertRequest, Guid userId)
    {
        Species? species = await _speciesRepository.GetSpeciesByIdAsync(createAlertRequest.SpeciesId);
        if (species is null)
        {
            throw new NotFoundException("Espécie com id especificado não existe.");
        }

        Breed? breed = null;
        if (createAlertRequest.BreedId is not null)
        {
            breed = await _breedRepository.GetBreedByIdAsync((int)createAlertRequest.BreedId);
            if (breed is null)
            {
                throw new NotFoundException("Raça com o id especificado não existe.");
            }
        }

        Guid foundAlertId = _guidProvider.NewGuid();
        
        string uploadedImageUrl =
            await _imageSubmissionService.UploadFoundAlertImageAsync(foundAlertId, createAlertRequest.Image);

        FoundAnimalAlert alertToBeCreated = new()
        {
            Id = foundAlertId,
            Name = createAlertRequest.Name,
            FoundLocationLatitude = createAlertRequest.FoundLocationLatitude,
            FoundLocationLongitude = createAlertRequest.FoundLocationLongitude,
            RegistrationDate = _dateTimeProvider.UtcNow(),
            HasBeenRecovered = false,
            Image = uploadedImageUrl,
            Species = species,
            Breed = breed
        };

        _foundAnimalAlertRepository.Add(alertToBeCreated);
        await _foundAnimalAlertRepository.CommitAsync();

        return alertToBeCreated.ToFoundAnimalAlertResponse();
    }
}