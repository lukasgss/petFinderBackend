using Application.Common.Interfaces.Entities.Breeds.DTOs;
using Application.Common.Interfaces.Entities.Colors.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Application.Common.Interfaces.Entities.Vaccines.DTOs;

namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class PetResponse
{
	public Guid Id { get; init; }
	public string Name { get; init; } = null!;
	public string? Observations { get; init; }
	public string Gender { get; init; } = null!;
	public required string Age { get; init; }
	public List<string> Images { get; init; } = null!;
	public OwnerResponse? Owner { get; init; }
	public required string Size { get; init; }
	public IEnumerable<ColorResponse> Colors { get; init; } = null!;
	public BreedResponse Breed { get; init; } = null!;
	public List<VaccineResponse> Vaccines { get; init; } = null!;
}