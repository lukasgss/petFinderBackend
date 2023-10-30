using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;


public class CreatePetRequest
{
    public string Name { get; set; } = null!;
    public string? Observations { get; set; }
    public Gender Gender { get; set; }
    public int? AgeInMonths { get; set; }
    public IFormFile Image { get; set; } = null!;
    public int BreedId { get; set; }
    public int SpeciesId { get; set; }
    public List<int> ColorIds { get; set; } = null!;
}