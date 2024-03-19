using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public class FoundAnimalAlertResponse
{
	public Guid Id { get; init; }
	public string? Name { get; init; }
	public string? Description { get; init; }
	public double FoundLocationLatitude { get; init; }
	public double FoundLocationLongitude { get; init; }
	public DateTime RegistrationDate { get; init; }
	public DateOnly? RecoveryDate { get; init; }
	public List<string> Images { get; init; } = null!;
	public required string Age { get; init; }
	public SpeciesResponse Species { get; init; } = null!;
	public BreedResponse? Breed { get; init; }
	public UserDataResponse Owner { get; init; } = null!;
	public string? Gender { get; init; }
	public required string Size { get; set; }
	public IEnumerable<ColorResponse> Colors { get; init; } = null!;
}