using Domain.Entities;

namespace Application.Common.Interfaces.General.UserPreferences;

public interface IUserPreferencesValidations
{
	Task<User> AssignUserAsync(Guid userId);
	Task<List<Breed>> ValidateAndAssignBreedAsync(List<int>? breedIds, List<Species> species);
	Task<List<Species>> ValidateAndAssignSpeciesAsync(List<int>? speciesIds);
	Task<List<Color>> ValidateAndAssignColorsAsync(List<int>? colorIds);
}