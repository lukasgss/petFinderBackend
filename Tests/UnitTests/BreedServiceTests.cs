using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.FrontendDropdownData;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using Tests.EntityGenerators;

namespace Tests.UnitTests;

public class BreedServiceTests
{
    private readonly IBreedRepository _breedRepositoryMock;
    private readonly IBreedService _sut;

    private const int _speciesId = 1;

    public BreedServiceTests()
    {
        _breedRepositoryMock = Substitute.For<IBreedRepository>();

        _sut = new BreedService(_breedRepositoryMock);
    }

    [Fact]
    public async Task Get_Breeds_For_Dropdown_With_Less_Than_2_Characters_Inserted_Throws_BadRequestException()
    {
        const string searchedName = "a";

        async Task Result() => await _sut.GetBreedsForDropdown(searchedName, _speciesId);

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Preencha no mínimo 2 caracteres para buscar a raça pelo nome.", exception.Message);
    }

    [Fact]
    public async Task Get_Breeds_For_Dropdown_Returns_Searched_Breeds()
    {
        const string breedName = "pug";

        List<Breed> returnedBreeds = BreedGenerator.GenerateListOfBreeds();
        _breedRepositoryMock.GetBreedsByNameAsync(breedName, _speciesId).Returns(returnedBreeds);
        IEnumerable<DropdownDataResponse<int>> expectedData = DropdownDataGenerator.GenerateListDropdownDataInt();

        IEnumerable<DropdownDataResponse<int>> breedData = await _sut.GetBreedsForDropdown(breedName, _speciesId);
        
        Assert.Equivalent(expectedData, breedData);
    }
}