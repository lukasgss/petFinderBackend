using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;

public class UserPreferencesResponse
{
	public required Guid Id { get; init; }
	public double? FoundLocationLatitude { get; init; }
	public double? FoundLocationLongitude { get; init; }
	public double? RadiusDistanceInKm { get; init; }
	public required List<string> Genders { get; init; }
	public required List<string> Ages { get; init; }
	public required List<string> Sizes { get; init; }
	public required List<SpeciesResponse> Species { get; init; }
	public required List<BreedResponse> Breeds { get; init; }
	public required List<ColorResponse> Colors { get; init; }
	public required UserDataResponse User { get; init; }
}