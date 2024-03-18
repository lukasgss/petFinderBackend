using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.UserPreferences;
using Domain.Entities;

namespace Application.Services.General.UserPreferences;

public class UserPreferencesValidations : IUserPreferencesValidations
{
	private readonly IUserRepository _userRepository;
	private readonly IBreedRepository _breedRepository;
	private readonly ISpeciesRepository _speciesRepository;
	private readonly IColorRepository _colorRepository;

	public UserPreferencesValidations(
		IUserRepository userRepository,
		IBreedRepository breedRepository,
		ISpeciesRepository speciesRepository,
		IColorRepository colorRepository)
	{
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_breedRepository = breedRepository ?? throw new ArgumentNullException(nameof(breedRepository));
		_speciesRepository = speciesRepository ?? throw new ArgumentNullException(nameof(speciesRepository));
		_colorRepository = colorRepository ?? throw new ArgumentNullException(nameof(colorRepository));
	}

	public async Task<User> AssignUserAsync(Guid userId)
	{
		return (await _userRepository.GetUserByIdAsync(userId))!;
	}

	public async Task<Breed?> ValidateAndAssignBreedAsync(int? breedId, int? speciesId)
	{
		if (breedId is null)
		{
			return null;
		}

		Breed? breed = await _breedRepository.GetBreedByIdAsync((int)breedId);
		if (breed is null)
		{
			throw new NotFoundException("Raça especificada não existe.");
		}

		if (breed.Species.Id != speciesId && speciesId is not null)
		{
			throw new BadRequestException("Raça não pertence a espécie especificada.");
		}

		return breed;
	}

	public async Task<Species?> ValidateAndAssignSpeciesAsync(int? speciesId)
	{
		if (speciesId is null)
		{
			return null;
		}

		Species? species = await _speciesRepository.GetSpeciesByIdAsync((int)speciesId);
		if (species is null)
		{
			throw new NotFoundException("Espécie especificada não existe.");
		}

		return species;
	}

	public async Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds)
	{
		if (!colorIds.Any())
		{
			return new List<Color>(0);
		}

		List<Color> colors = await _colorRepository.GetMultipleColorsByIdsAsync(colorIds);
		if (colors.Count != colorIds.Count || colors.Count == 0)
		{
			throw new NotFoundException("Alguma das cores especificadas não existe.");
		}

		return colors;
	}
}