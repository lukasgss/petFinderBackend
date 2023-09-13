using System.Collections.Generic;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using Tests.EntityGenerators;

namespace Tests.UnitTests;

public class SpeciesServiceTests
{
    private readonly ISpeciesRepository _speciesRepositoryMock;
    private readonly ISpeciesService _sut;

    private static readonly List<Species> Species = SpeciesGenerator.GenerateListOfSpecies();

    private static readonly List<DropdownDataResponse<int>> ExpectedDropdownData =
        DropdownDataGenerator.GenerateDropdownDataResponsesOfSpecies(Species);

    public SpeciesServiceTests()
    {
        _speciesRepositoryMock = Substitute.For<ISpeciesRepository>();

        _sut = new SpeciesService(_speciesRepositoryMock);
    }

    [Fact]
    public async Task Get_All_Species_For_Dropdown_Returns_All_Species()
    {
        _speciesRepositoryMock.GetAllSpecies().Returns(Species);

        var dropdownData = await _sut.GetAllSpeciesForDropdown();

        Assert.Equivalent(ExpectedDropdownData, dropdownData);
    }
}