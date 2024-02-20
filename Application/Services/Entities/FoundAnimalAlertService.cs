using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
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
	private readonly IUserRepository _userRepository;
	private readonly IColorRepository _colorRepository;
	private readonly IImageSubmissionService _imageSubmissionService;
	private readonly IValueProvider _valueProvider;

	public FoundAnimalAlertService(
		IFoundAnimalAlertRepository foundAnimalAlertRepository,
		ISpeciesRepository speciesRepository,
		IBreedRepository breedRepository,
		IUserRepository userRepository,
		IColorRepository colorRepository,
		IImageSubmissionService imageSubmissionService,
		IValueProvider valueProvider)
	{
		_foundAnimalAlertRepository = foundAnimalAlertRepository ??
		                              throw new ArgumentNullException(nameof(foundAnimalAlertRepository));
		_imageSubmissionService = imageSubmissionService ??
		                          throw new ArgumentNullException(nameof(imageSubmissionService));
		_colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
		_speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
		_breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<FoundAnimalAlertResponse> GetByIdAsync(Guid alertId)
	{
		FoundAnimalAlert? foundAnimalAlert = await _foundAnimalAlertRepository.GetByIdAsync(alertId);
		if (foundAnimalAlert is null)
		{
			throw new NotFoundException("Alerta com o id especificado não existe.");
		}

		return foundAnimalAlert.ToFoundAnimalAlertResponse();
	}

	public async Task<FoundAnimalAlertResponse> CreateAsync(CreateFoundAnimalAlertRequest createAlertRequest,
		Guid userId)
	{
		Species? species = await _speciesRepository.GetSpeciesByIdAsync(createAlertRequest.SpeciesId);
		if (species is null)
		{
			throw new NotFoundException("Espécie com o id especificado não existe.");
		}

		List<Color> colors = await ValidateAndAssignColorsAsync(createAlertRequest.ColorIds);
		Breed? breed = await ValidateAndQueryBreed(createAlertRequest.BreedId);
		User? userCreating = await _userRepository.GetUserByIdAsync(userId);
		Guid foundAlertId = _valueProvider.NewGuid();

		string uploadedImageUrl =
			await _imageSubmissionService.UploadFoundAlertImageAsync(foundAlertId, createAlertRequest.Image);

		FoundAnimalAlert alertToBeCreated = new()
		{
			Id = foundAlertId,
			Name = createAlertRequest.Name,
			Description = createAlertRequest.Description,
			FoundLocationLatitude = createAlertRequest.FoundLocationLatitude,
			FoundLocationLongitude = createAlertRequest.FoundLocationLongitude,
			RegistrationDate = _valueProvider.UtcNow(),
			HasBeenRecovered = false,
			Image = uploadedImageUrl,
			Gender = createAlertRequest.Gender,
			Colors = colors,
			Species = species,
			Breed = breed,
			User = userCreating!
		};

		_foundAnimalAlertRepository.Add(alertToBeCreated);
		await _foundAnimalAlertRepository.CommitAsync();

		return alertToBeCreated.ToFoundAnimalAlertResponse();
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
			throw new NotFoundException("Raça com o id especificado não existe.");
		}

		return breed;
	}

	private async Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds)
	{
		List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
		if (colors.Count != colorIds.Count || colors.Count == 0)
		{
			throw new NotFoundException("Alguma das cores especificadas não existe.");
		}

		return colors;
	}
}