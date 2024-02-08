using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;

public class FoundAnimalAlertResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double FoundLocationLatitude { get; set; }
    public double FoundLocationLongitude { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool HasBeenRecovered { get; set; }
    public string Image { get; set; } = null!;
    public SpeciesResponse Species { get; set; } = null!;
    public BreedResponse? Breed { get; set; } = null!;
    public UserDataResponse Owner { get; set; } = null!;
    public string? Gender { get; set; }
    public IEnumerable<ColorResponse> Colors { get; set; } = null!;
}