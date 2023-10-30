using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class PetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Observations { get; set; }
    public string Gender { get; set; } = null!;
    public int? AgeInMonths { get; set; }
    public string Image { get; set; } = null!;
    public OwnerResponse? Owner { get; set; }
    public IEnumerable<ColorResponse> Colors { get; set; } = null!;
    public BreedResponse Breed { get; set; } = null!;
}