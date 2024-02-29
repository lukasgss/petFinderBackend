using Application.Common.Interfaces.Entities.Vaccines.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class VaccineMappings
{
	private static VaccineResponse ToVaccineResponse(this Vaccine vaccine)
	{
		return new VaccineResponse()
		{
			Name = vaccine.Name
		};
	}

	public static List<VaccineResponse> ToVaccineResponseList(this IEnumerable<Vaccine> vaccines)
	{
		List<VaccineResponse> vaccineResponses = new();
		foreach (Vaccine vaccine in vaccines)
		{
			vaccineResponses.Add(vaccine.ToVaccineResponse());
		}

		return vaccineResponses;
	}
}