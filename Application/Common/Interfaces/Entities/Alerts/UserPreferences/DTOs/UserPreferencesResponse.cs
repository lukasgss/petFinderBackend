using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;

public class UserPreferencesResponse
{
	public required Guid Id { get; set; }

	public double? FoundLocationLatitude { get; set; }

	public double? FoundLocationLongitude { get; set; }

	public double? RadiusDistanceInKm { get; set; }

	public string? Gender { get; set; }

	public SpeciesResponse? Species { get; set; }

	public virtual BreedResponse? Breed { get; set; }

	public virtual required UserDataResponse User { get; set; } = null!;

	public required IEnumerable<ColorResponse> Colors { get; set; } = null!;
}