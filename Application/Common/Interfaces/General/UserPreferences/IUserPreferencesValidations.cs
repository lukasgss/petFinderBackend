using Domain.Entities;

namespace Application.Common.Interfaces.General.UserPreferences;

public interface IUserPreferencesValidations
{
	Task<User> AssignUserAsync(Guid userId);
	Task<Breed?> ValidateAndAssignBreedAsync(int? breedId, int? speciesId);
	Task<Species?> ValidateAndAssignSpeciesAsync(int? speciesId);
	Task<Age?> ValidateAndAssignAgeAsync(int? ageId);
	Task<List<Color>> ValidateAndAssignColorsAsync(List<int> colorIds);
}