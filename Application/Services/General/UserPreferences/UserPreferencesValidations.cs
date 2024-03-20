using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.UserPreferences;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.General.UserPreferences;

public class UserPreferencesValidations : IUserPreferencesValidations
{
	private readonly IUserRepository _userRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;
	private readonly ILogger<UserPreferencesValidations> _logger;

	public UserPreferencesValidations(
		IUserRepository userRepository,
		IBreedRepository breedRepository,
		ISpeciesRepository speciesRepository,
		IColorRepository colorRepository,
		ILogger<UserPreferencesValidations> logger)
	{
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
		_speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
		_colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<User> AssignUserAsync(Guid userId)
	{
		return (await _userRepository.GetUserByIdAsync(userId))!;
	}

	public async Task<List<Breed>> ValidateAndAssignBreedAsync(List<int>? breedIds, List<Species> species)
	{
		if (breedIds is null || !breedIds.Any())
		{
			return new List<Breed>(0);
		}

		var breeds = await _breedRepository.GetMultipleBreedsByIdAsync(breedIds);
		if (breeds.Count != breedIds.Count || !breeds.Any())
		{
			_logger.LogInformation("Alguma das raças {@BreedIds} não existe", breedIds);
			throw new NotFoundException("Alguma das raças especificadas não existe.");
		}

		return breeds;
	}

	public async Task<List<Species>> ValidateAndAssignSpeciesAsync(List<int>? speciesIds)
	{
		if (speciesIds is null)
		{
			return new List<Species>(0);
		}

		var species = await _speciesRepository.GetMultipleSpeciesByIdAsync(speciesIds);
		if (species.Count != speciesIds.Count || species.Count == 0)
		{
			_logger.LogInformation("Alguma das espécies {@SpeciesIds} não existe", speciesIds);
			throw new NotFoundException("Alguma das espécies especificadas não existe.");
		}

		return species;
	}

	public async Task<List<Color>> ValidateAndAssignColorsAsync(List<int>? colorIds)
	{
		if (colorIds is null || !colorIds.Any())
		{
			return new List<Color>(0);
		}

		List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
		if (colors.Count != colorIds.Count || colors.Count == 0)
		{
			_logger.LogInformation("Alguma das cores {@ColorIds} não existe", colorIds);
			throw new NotFoundException("Alguma das cores especificadas não existe.");
		}

		return colors;
	}
}