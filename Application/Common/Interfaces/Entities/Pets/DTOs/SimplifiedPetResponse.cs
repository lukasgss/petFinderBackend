namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class SimplifiedPetResponse
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }
	public string? Observations { get; init; }
	public required string Gender { get; init; }
	public int? AgeInMonths { get; init; }
	public required List<string> Images { get; init; }
}