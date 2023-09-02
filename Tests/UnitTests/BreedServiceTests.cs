using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using Tests.EntityGenerators;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.UnitTests;

public class BreedServiceTests
{
    private readonly IBreedRepository _breedRepositoryMock;
    private readonly IBreedService _sut;

    public BreedServiceTests()
    {
        _breedRepositoryMock = Substitute.For<IBreedRepository>();

        _sut = new BreedService(_breedRepositoryMock);
    }

    [Fact]
    public async Task Get_Breeds_For_Dropdown_With_Less_Than_2_Characters_Inserted_Throws_BadRequestException()
    {
        const string searchedName = "a";

        async Task Result() => await _sut.GetBreedsForDropdown(searchedName, Constants.SpeciesData.Id);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Preencha no mínimo 2 caracteres para buscar a raça pelo nome.", exception.Message);
    }

    [Fact]
    public async Task Get_Breeds_For_Dropdown_Returns_Searched_Breeds()
    {
        List<Breed> returnedBreeds = BreedGenerator.GenerateListOfBreeds();
        _breedRepositoryMock.GetBreedsByNameAsync(Constants.BreedData.Name, Constants.SpeciesData.Id)
            .Returns(returnedBreeds);
        IEnumerable<DropdownDataResponse<int>> expectedData =
            DropdownDataGenerator.GenerateDropdownDataResponsesOfBreeds(returnedBreeds);

        IEnumerable<DropdownDataResponse<int>> breedData =
            await _sut.GetBreedsForDropdown(Constants.BreedData.Name, Constants.SpeciesData.Id);

        Assert.Equivalent(expectedData, breedData);
    }
}