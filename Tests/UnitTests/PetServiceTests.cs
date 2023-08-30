using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;

namespace Tests.UnitTests;

public class PetServiceTests
{
    private readonly IPetRepository _petRepositoryMock;
    private readonly IBreedRepository _breedRepositoryMock;
    private readonly ISpeciesRepository _speciesRepositoryMock;
    private readonly IColorRepository _colorRepositoryMock;
    private readonly IGuidProvider _guidProviderMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPetService _sut;

    private readonly Guid _petId = Guid.NewGuid();
    private readonly string _name = "petName";
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly int _breedId = 1;
    private readonly int _speciesId = 1;
    private readonly int[] _colorIds = new[] { 1, 3 };


    public PetServiceTests()
    {
        _petRepositoryMock = Substitute.For<IPetRepository>();
        _breedRepositoryMock = Substitute.For<IBreedRepository>();
        _speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
        _colorRepositoryMock = Substitute.For<IColorRepository>();
        _guidProviderMock = Substitute.For<IGuidProvider>();
        _userRepositoryMock = Substitute.For<IUserRepository>();

        _sut = new PetService(
            _petRepositoryMock,
            _breedRepositoryMock,
            _speciesRepositoryMock,
            _colorRepositoryMock,
            _guidProviderMock,
            _userRepositoryMock);
    }

    [Fact]
    public async Task Get_Non_Existent_Pet_By_Id_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

        async Task Result() => await _sut.GetPetBydIdAsync(Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Pet_By_Id_Returns_PetResponse()
    {
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(Arg.Any<Guid>()).Returns(pet);
        PetResponse expectedPetResponse = pet.ConvertToPetResponse(
            pet.Owner!.ConvertToOwnerResponse(),
            pet.Colors.ConvertToListOfColorResponse(),
            pet.Breed.ConvertToBreedResponse());

        PetResponse petResponse = await _sut.GetPetBydIdAsync(_petId);

        Assert.Equivalent(expectedPetResponse, petResponse);
    }

    [Fact]
    public async Task Create_Pet_With_Non_Existent_Breed_Throws_NotFoundException()
    {
        CreatePetRequest createPetRequest = PetGenerator.GenerateCreatePetRequest();
        _breedRepositoryMock.GetBreedByIdAsync(createPetRequest.BreedId).ReturnsNull();

        async Task Result() => await _sut.CreatePetAsync(createPetRequest, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Raça especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Pet_With_Non_Existent_Species_Throws_NotFoundException()
    {
        CreatePetRequest createPetRequest = PetGenerator.GenerateCreatePetRequest();
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(createPetRequest.BreedId).Returns(breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(createPetRequest.SpeciesId).ReturnsNull();

        async Task Result() => await _sut.CreatePetAsync(createPetRequest, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Espécie especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Pet_With_Non_Existent_Colors_Throws_NotFoundException()
    {
        CreatePetRequest createPetRequest = PetGenerator.GenerateCreatePetRequest();
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(createPetRequest.BreedId).Returns(breed);
        Species species = SpeciesGenerator.GenerateSpecies();
        _speciesRepositoryMock.GetSpeciesByIdAsync(createPetRequest.SpeciesId).Returns(species);
        List<Color> emptyColorList = new();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(createPetRequest.ColorIds).Returns(emptyColorList);

        async Task Result() => await _sut.CreatePetAsync(createPetRequest, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
    }

    [Fact]
    public async Task Create_Pet_While_Unauthenticated_Returns_PetResponse_With_Null_Owner()
    {
        // Arrange
        CreatePetRequest createPetRequest = PetGenerator.GenerateCreatePetRequest();
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(createPetRequest.BreedId).Returns(breed);
        Species species = SpeciesGenerator.GenerateSpecies();
        _speciesRepositoryMock.GetSpeciesByIdAsync(createPetRequest.SpeciesId).Returns(species);
        List<Color> colors = ColorGenerator.GenerateListOfColors();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(createPetRequest.ColorIds).Returns(colors);
        _guidProviderMock.NewGuid().Returns(_petId);

        Pet petToBeCreated = PetGenerator.GeneratePetFromCreatePetRequest(
            createPetRequest: createPetRequest,
            petId: _petId,
            owner: null,
            breed: breed,
            species: species,
            colors: colors);
        PetResponse expectedPetResponse = petToBeCreated.ConvertToPetResponse(
            owner: null,
            colors.ConvertToListOfColorResponse(),
            breed.ConvertToBreedResponse());

        // Act
        PetResponse petResponse = await _sut.CreatePetAsync(createPetRequest, userId: null);

        // Assert
        Assert.Equivalent(expectedPetResponse, petResponse);
    }

    [Fact]
    public async Task Create_Pet_While_Authenticated_Returns_Pet_Response_With_Logged_In_User_As_owner()
    {
        // Arrange
        CreatePetRequest createPetRequest = PetGenerator.GenerateCreatePetRequest();
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(createPetRequest.BreedId).Returns(breed);
        Species species = SpeciesGenerator.GenerateSpecies();
        _speciesRepositoryMock.GetSpeciesByIdAsync(createPetRequest.SpeciesId).Returns(species);
        List<Color> colors = ColorGenerator.GenerateListOfColors();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(createPetRequest.ColorIds).Returns(colors);
        User loggedInUser = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(_userId).Returns(loggedInUser);
        _guidProviderMock.NewGuid().Returns(_petId);

        Pet petToBeCreated = PetGenerator.GeneratePetFromCreatePetRequest(
            createPetRequest: createPetRequest,
            petId: _petId,
            owner: loggedInUser,
            breed: breed,
            species: species,
            colors: colors);

        PetResponse expectedPetResponse = petToBeCreated.ConvertToPetResponse(
            owner: loggedInUser.ConvertToOwnerResponse(),
            colors.ConvertToListOfColorResponse(),
            breed.ConvertToBreedResponse());

        // Act
        PetResponse petResponse = await _sut.CreatePetAsync(createPetRequest, userId: _userId);

        // Assert
        Assert.Equivalent(expectedPetResponse, petResponse);
    }

    [Fact]
    public async Task Edit_Pet_With_Route_Id_Different_Than_Specified_On_Request_Throws_BadRequestException()
    {
        EditPetRequest editPetRequest = PetGenerator.GenerateEditPetRequest();

        async Task Result() => await _sut.EditPetAsync(editPetRequest, _userId, routeId: Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
        Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
    }

    [Fact]
    public async Task Edit_Non_Existent_Pet_Throws_NotFoundException()
    {
        EditPetRequest editPetRequest = PetGenerator.GenerateEditPetRequest();
        _petRepositoryMock.GetPetByIdAsync(_petId).ReturnsNull();

        async Task Result() => await _sut.EditPetAsync(editPetRequest, _userId, editPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_With_Non_Existent_Breed_Throws_NotFoundException()
    {
        EditPetRequest editPetRequest = PetGenerator.GenerateEditPetRequest();
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editPetRequest.Id).Returns(pet);
        _breedRepositoryMock.GetBreedByIdAsync(editPetRequest.BreedId).ReturnsNull();

        async Task Result() => await _sut.EditPetAsync(editPetRequest, _userId, editPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Raça especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_With_Non_Existent_Species_Throws_NotFoundException()
    {
        EditPetRequest editPetRequest = PetGenerator.GenerateEditPetRequest();
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editPetRequest.Id).Returns(pet);
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(breed.Id).Returns(breed);
        _speciesRepositoryMock.GetSpeciesByIdAsync(editPetRequest.SpeciesId).ReturnsNull();

        async Task Result() => await _sut.EditPetAsync(editPetRequest, _userId, editPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Espécie especificada não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_With_Non_Existent_Colors_Throws_NotFoundException()
    {
        EditPetRequest editPetRequest = PetGenerator.GenerateEditPetRequest();
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editPetRequest.Id).Returns(pet);
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(breed.Id).Returns(breed);
        Species species = SpeciesGenerator.GenerateSpecies();
        _speciesRepositoryMock.GetSpeciesByIdAsync(species.Id).Returns(species);
        List<Color> emptyColorsList = new();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(editPetRequest.ColorIds).Returns(emptyColorsList);

        async Task Result() => await _sut.EditPetAsync(editPetRequest, _userId, editPetRequest.Id);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
    }

    [Fact]
    public async Task Edit_Pet_Without_Being_Owner_Throws_UnauthorizedException()
    {
        EditPetRequest editPetRequest = PetGenerator.GenerateEditPetRequest();
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(editPetRequest.Id).Returns(pet);
        Breed breed = BreedGenerator.GenerateBreed();
        _breedRepositoryMock.GetBreedByIdAsync(breed.Id).Returns(breed);
        Species species = SpeciesGenerator.GenerateSpecies();
        _speciesRepositoryMock.GetSpeciesByIdAsync(species.Id).Returns(species);
        List<Color> colors = ColorGenerator.GenerateListOfColors();
        _colorRepositoryMock.GetMultipleColorsByIdsAsync(editPetRequest.ColorIds).Returns(colors);
        User user = UserGenerator.GenerateUser();
        _userRepositoryMock.GetUserByIdAsync(_userId).Returns(user);

        async Task Result() => await _sut.EditPetAsync(editPetRequest, _userId, editPetRequest.Id);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Você não possui permissão para editar dados desse animal.", exception.Message);
    }

    [Fact]
    public async Task Delete_Non_Existent_Pet_Throws_NotFoundException()
    {
        _petRepositoryMock.GetPetByIdAsync(_petId).ReturnsNull();

        async Task Result() => await _sut.DeletePetAsync(_petId, _userId);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Animal especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Delete_Pet_Without_Assigned_Owner_Throws_UnauthorizedException()
    {
        Pet petWithoutOwner = PetGenerator.GeneratePetWithoutOwner();
        _petRepositoryMock.GetPetByIdAsync(_petId).Returns(petWithoutOwner);

        async Task Result() => await _sut.DeletePetAsync(_petId, _userId);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Você não possui permissão para excluir o animal.", exception.Message);
    }

    [Fact]
    public async Task Delete_Pet_Without_Being_Its_Owner_Throws_UnauthorizedException()
    {
        Pet pet = PetGenerator.GeneratePet();
        _petRepositoryMock.GetPetByIdAsync(_petId).Returns(pet);

        async Task Result() => await _sut.DeletePetAsync(_petId, _userId);

        var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
        Assert.Equal("Você não possui permissão para excluir o animal.", exception.Message);
    }
}