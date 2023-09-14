using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class PetResponseNoOwner
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Observations { get; set; }
    public Gender Gender { get; set; }
    public int? AgeInMonths { get; set; }
    public IEnumerable<ColorResponse> Colors { get; set; } = null!;
    public BreedResponse Breed { get; set; } = null!;
}