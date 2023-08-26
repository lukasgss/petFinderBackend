namespace Application.Common.Interfaces.Entities.Pets.DTOs;


public class CreatePetRequest
{
    public string Name { get; set; } = null!;
    public string? Observations { get; set; }
    public int BreedId { get; set; }
    public int SpeciesId { get; set; }
    public List<int> ColorIds { get; set; } = null!;
}