using Application.Common.Interfaces.Entities.Vaccines.DTOs;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class VaccineMappings
{
	public static List<VaccineResponse> ToVaccineResponseList(this IEnumerable<Vaccine> vaccines)
	{
		return vaccines.Select(vaccine => vaccine.ToVaccineResponse()).ToList();
	}

	public static List<DropdownDataResponse<int>> ToVaccineDropdownResponseList(this IEnumerable<Vaccine> vaccines)
	{
		return vaccines.Select(vaccine => new DropdownDataResponse<int>() { Text = vaccine.Name, Value = vaccine.Id })
			.ToList();
	}

	private static VaccineResponse ToVaccineResponse(this Vaccine vaccine)
	{
		return new VaccineResponse()
		{
			Name = vaccine.Name
		};
	}
}