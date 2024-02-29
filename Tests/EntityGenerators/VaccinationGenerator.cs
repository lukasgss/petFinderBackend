using System.Collections.Generic;
using Application.Common.Interfaces.Entities.Pets.DTOs;

namespace Tests.EntityGenerators;

public static class VaccinationGenerator
{
	public static PetVaccinationRequest GeneratePetVaccinationRequest()
	{
		return new PetVaccinationRequest()
		{
			VaccinationIds = new List<int>() { 1 }
		};
	}
}