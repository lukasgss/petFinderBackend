using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Services.General.UserPreferences;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;

namespace Tests.UnitTests;

public class UserPreferencesValidationsTests
{
	private readonly IUserRepository _userRepositoryMock;
	private readonly IBreedRepository _breedRepositoryMock;
	private readonly ISpeciesRepository _speciesRepositoryMock;
	private readonly IColorRepository _colorRepositoryMock;
	private readonly IUserPreferencesValidations _sut;

	private static readonly User User = UserGenerator.GenerateUser();
	private static readonly Breed Breed = BreedGenerator.GenerateBreed();
	private static readonly Species Species = SpeciesGenerator.GenerateSpecies();
	private static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
	private static readonly List<int> ColorIds = new() { 1 };

	public UserPreferencesValidationsTests()
	{
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_breedRepositoryMock = Substitute.For<IBreedRepository>();
		_speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
		_colorRepositoryMock = Substitute.For<IColorRepository>();
		_sut = new UserPreferencesValidations(
			_userRepositoryMock,
			_breedRepositoryMock,
			_speciesRepositoryMock,
			_colorRepositoryMock);
	}

	[Fact]
	public async Task Assign_User_Returns_Assigned_User()
	{
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);

		User returnedUser = await _sut.AssignUserAsync(User.Id);

		Assert.Equivalent(User, returnedUser);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Returns_Null_If_Breed_Id_Is_Null()
	{
		Breed? returnedBreed = await _sut.ValidateAndAssignBreedAsync(breedId: null, Species.Id);

		Assert.Null(returnedBreed);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Throws_NotFoundException_If_Breed_Is_NonExistent()
	{
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).ReturnsNull();

		async Task Result() => await _sut.ValidateAndAssignBreedAsync(Breed.Id, Species.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Raça especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Throws_BadRequestException_If_Breed_Does_Not_Belong_To_Species()
	{
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);
		const int differentSpeciesId = 99;

		async Task Result() => await _sut.ValidateAndAssignBreedAsync(Breed.Id, differentSpeciesId);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Raça não pertence a espécie especificada.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Returns_Breed_If_Species_Is_Null()
	{
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);

		Breed? returnedBreed = await _sut.ValidateAndAssignBreedAsync(Breed.Id, null);

		Assert.Equivalent(Breed, returnedBreed);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Returns_Breed_If_Breed_Is_From_Species()
	{
		_breedRepositoryMock.GetBreedByIdAsync(Breed.Id).Returns(Breed);

		Breed? returnedBreed = await _sut.ValidateAndAssignBreedAsync(Breed.Id, Species.Id);

		Assert.Equivalent(Breed, returnedBreed);
	}

	[Fact]
	public async Task Validate_And_Assign_Species_Returns_Null_If_Species_Id_Is_Null()
	{
		Species? returnedSpecies = await _sut.ValidateAndAssignSpeciesAsync(null);

		Assert.Null(returnedSpecies);
	}

	[Fact]
	public async Task Validate_And_Assign_Species_Throws_NotFoundException_If_Species_Does_Not_Exist()
	{
		_speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).ReturnsNull();

		async Task Result() => await _sut.ValidateAndAssignSpeciesAsync(Species.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Espécie especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Species_Returns_Species()
	{
		_speciesRepositoryMock.GetSpeciesByIdAsync(Species.Id).Returns(Species);

		Species? returnedSpecies = await _sut.ValidateAndAssignSpeciesAsync(Species.Id);

		Assert.Equivalent(Species, returnedSpecies);
	}

	[Fact]
	public async Task Validate_And_Assign_Colors_Returns_Empty_List_If_Color_Ids_List_Is_Empty()
	{
		List<Color> emptyColorsList = new(0);

		List<Color> returnedColors = await _sut.ValidateAndAssignColorsAsync(new List<int>());

		Assert.Equivalent(emptyColorsList, returnedColors);
	}

	[Fact]
	public async Task Validate_And_Assign_Colors_Throws_NotFoundException_When_Not_All_Colors_Are_Found()
	{
		List<Color> differentColorsList = new()
		{
			new Color(), new Color(), new Color()
		};
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(ColorIds).Returns(differentColorsList);

		async Task Result() => await _sut.ValidateAndAssignColorsAsync(ColorIds);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Colors_Throws_NotFoundException_When_No_Colors_Are_Found()
	{
		List<Color> emptyColorsList = new();
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(ColorIds).Returns(emptyColorsList);

		async Task Result() => await _sut.ValidateAndAssignColorsAsync(ColorIds);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Colors_Returns_Colors()
	{
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(ColorIds).Returns(Colors);

		List<Color> returnedColors = await _sut.ValidateAndAssignColorsAsync(ColorIds);

		Assert.Equivalent(Colors, returnedColors);
	}
}