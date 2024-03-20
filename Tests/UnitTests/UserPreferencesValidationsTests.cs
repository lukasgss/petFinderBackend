using System.Collections.Generic;
using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Services.General.UserPreferences;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
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
	private static readonly List<Breed> Breeds = BreedGenerator.GenerateListOfBreeds();
	private static readonly List<Species> SpeciesList = SpeciesGenerator.GenerateListOfSpecies();
	private static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
	private static readonly List<int> ColorIds = new() { 1 };
	private static readonly List<int>? BreedIds = Breeds.Select(breed => breed.Id).ToList();
	private static readonly List<int> SpeciesIds = SpeciesList.Select(species => species.Id).ToList();

	public UserPreferencesValidationsTests()
	{
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_breedRepositoryMock = Substitute.For<IBreedRepository>();
		_speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
		_colorRepositoryMock = Substitute.For<IColorRepository>();
		var loggerMock = Substitute.For<ILogger<UserPreferencesValidations>>();
		_sut = new UserPreferencesValidations(
			_userRepositoryMock,
			_breedRepositoryMock,
			_speciesRepositoryMock,
			_colorRepositoryMock,
			loggerMock);
	}

	[Fact]
	public async Task Assign_User_Returns_Assigned_User()
	{
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);

		User returnedUser = await _sut.AssignUserAsync(User.Id);

		Assert.Equivalent(User, returnedUser);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Returns_Empty_If_Breed_Ids_Are_Null()
	{
		List<Breed> emptyList = new(0);

		List<Breed> returnedBreed = await _sut.ValidateAndAssignBreedAsync(breedIds: null, SpeciesList);

		Assert.Equivalent(emptyList, returnedBreed);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Throws_NotFoundException_If_Any_Breed_Is_NonExistent()
	{
		List<Breed> emptyBreedList = new(0);

		_breedRepositoryMock.GetMultipleBreedsByIdAsync(BreedIds!).Returns(emptyBreedList);

		async Task Result() => await _sut.ValidateAndAssignBreedAsync(BreedIds, SpeciesList);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das raças especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Breed_Returns_Breeds()
	{
		_breedRepositoryMock.GetMultipleBreedsByIdAsync(BreedIds!).Returns(Breeds);

		var returnedBreeds = await _sut.ValidateAndAssignBreedAsync(BreedIds, SpeciesList);

		Assert.Equivalent(Breeds, returnedBreeds);
	}

	[Fact]
	public async Task Validate_And_Assign_Species_Returns_Empty_List_If_Species_Id_Is_Null()
	{
		List<Species> emptyList = new(0);

		var returnedSpecies = await _sut.ValidateAndAssignSpeciesAsync(null);

		Assert.Equivalent(emptyList, returnedSpecies);
	}

	[Fact]
	public async Task Validate_And_Assign_Species_Throws_NotFoundException_If_Species_Does_Not_Exist()
	{
		List<Species> differentList = new List<Species>(0);

		_speciesRepositoryMock.GetMultipleSpeciesByIdAsync(SpeciesIds).Returns(differentList);

		async Task Result() => await _sut.ValidateAndAssignSpeciesAsync(SpeciesIds);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das espécies especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Validate_And_Assign_Species_Returns_Species()
	{
		_speciesRepositoryMock.GetMultipleSpeciesByIdAsync(SpeciesIds).Returns(SpeciesList);

		var returnedSpecies = await _sut.ValidateAndAssignSpeciesAsync(SpeciesIds);

		Assert.Equivalent(SpeciesList, returnedSpecies);
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