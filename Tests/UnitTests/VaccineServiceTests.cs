using System.Collections.Generic;
using Application.Common.Interfaces.Entities.Vaccines;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;

namespace Tests.UnitTests;

public class VaccineServiceTests
{
	private readonly IVaccineRepository _vaccineRepositoryMock;
	private readonly IVaccineService _sut;

	private const int SpeciesId = 1;
	private const int VaccineId = 1;
	private const string NomeVacina = "Antirrábica";

	public VaccineServiceTests()
	{
		_vaccineRepositoryMock = Substitute.For<IVaccineRepository>();

		_sut = new VaccineService(_vaccineRepositoryMock);
	}

	[Fact]
	public async Task Get_Vaccines_Of_Species_Returns_Species()
	{
		List<Vaccine> vaccines = new()
		{
			new Vaccine() { Id = VaccineId, Name = NomeVacina }
		};
		List<DropdownDataResponse<int>> expectedResponse = new()
		{
			new DropdownDataResponse<int>() { Text = NomeVacina, Value = VaccineId }
		};
		_vaccineRepositoryMock.GetVaccinesOfSpecies(SpeciesId).Returns(vaccines);

		var vaccineResponse = await _sut.GetVaccinesOfSpecies(SpeciesId);

		Assert.Equivalent(expectedResponse, vaccineResponse);
	}
}