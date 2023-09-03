using System.Collections.Generic;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using Tests.EntityGenerators;

namespace Tests.UnitTests;

public class SpeciesServiceTests
{
    private readonly ISpeciesRepository _speciesRepositoryMock;
    private readonly ISpeciesService _sut;

    public SpeciesServiceTests()
    {
        _speciesRepositoryMock = Substitute.For<ISpeciesRepository>();

        _sut = new SpeciesService(_speciesRepositoryMock);
    }

    [Fact]
    public async Task Get_All_Species_For_Dropdown_Returns_All_Species()
    {
        List<Species> species = SpeciesGenerator.GenerateListOfSpecies();
        _speciesRepositoryMock.GetAllSpecies().Returns(species);
        var expectedDropdownData = DropdownDataGenerator.GenerateDropdownDataResponsesOfSpecies(species);

        var dropdownData = await _sut.GetAllSpeciesForDropdown();
        
        Assert.Equivalent(expectedDropdownData, dropdownData);
    }
}