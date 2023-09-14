using Domain.Enums;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class EditPetRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Observations { get; set; }
    public Gender Gender { get; set; }
    public int? AgeInMonths { get; set; }
    public int BreedId { get; set; }
    public int SpeciesId { get; set; }
    public List<int> ColorIds { get; set; } = null!;
}