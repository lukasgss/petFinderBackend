namespace Application.Common.Interfaces.Entities.Pets.DTOs;

public class PetVaccinationRequest
{
	public List<int> VaccinationIds { get; set; } = null!;
}