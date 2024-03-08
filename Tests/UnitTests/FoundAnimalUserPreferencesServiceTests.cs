using System.Collections.Generic;
using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts.UserPreferences;

namespace Tests.UnitTests;

public class FoundAnimalUserPreferencesServiceTests
{
	private readonly IFoundAnimalUserPreferencesRepository _foundAnimalUserPreferencesRepositoryMock;
	private readonly IBreedRepository _breedRepositoryMock;
	private readonly ISpeciesRepository _speciesRepositoryMock;
	private readonly IColorRepository _colorRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IFoundAnimalUserPreferencesService _sut;

	private static readonly FoundAnimalUserPreferences UserPreferences =
		FoundAnimalUserPreferencesGenerator.GenerateFoundAnimalUserPreferences();

	private static readonly FoundAnimalUserPreferencesResponse ExpectedUserPreferencesResponse =
		FoundAnimalUserPreferencesGenerator.GenerateFoundAnimalUserPreferences()
			.ToFoundAnimalUserPreferencesResponse();

	private static readonly CreateFoundAnimalUserPreferences CreateFoundAnimalUserPreferences =
		FoundAnimalUserPreferencesGenerator.GenerateCreateFoundAnimalUserPreferences();

	private static readonly User User = UserGenerator.GenerateUser();

	public FoundAnimalUserPreferencesServiceTests()
	{
		_foundAnimalUserPreferencesRepositoryMock = Substitute.For<IFoundAnimalUserPreferencesRepository>();
		_breedRepositoryMock = Substitute.For<IBreedRepository>();
		_speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
		_colorRepositoryMock = Substitute.For<IColorRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		_sut = new FoundAnimalUserPreferencesService(_foundAnimalUserPreferencesRepositoryMock,
			_breedRepositoryMock,
			_speciesRepositoryMock,
			_colorRepositoryMock,
			_userRepositoryMock,
			_valueProviderMock);
	}

	[Fact]
	public async Task Get_User_Preferences_Of_User_With_No_Preferences_Throws_NotFoundException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();

		async Task Result() => await _sut.GetUserPreferences(User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Ainda não foram definidas preferências para este usuário.", exception.Message);
	}

	[Fact]
	public async Task Get_User_Preferences_Returns_User_Preferences()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).Returns(UserPreferences);

		FoundAnimalUserPreferencesResponse userPreferencesResponse = await _sut.GetUserPreferences(User.Id);

		Assert.Equivalent(ExpectedUserPreferencesResponse, userPreferencesResponse);
	}

	[Fact]
	public async Task Create_User_Preferences_When_Already_Registered_Throws_BadRequestException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).Returns(UserPreferences);

		async Task Result() => await _sut.CreatePreferences(CreateFoundAnimalUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Usuário já possui preferências cadastradas.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_With_Non_Existent_Species_Throws_NotFoundException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_speciesRepositoryMock.GetSpeciesByIdAsync((int)CreateFoundAnimalUserPreferences.SpeciesId!).ReturnsNull();

		async Task Result() => await _sut.CreatePreferences(CreateFoundAnimalUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Espécie especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_With_Non_Existent_Breed_Throws_NotFoundException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_speciesRepositoryMock.GetSpeciesByIdAsync((int)CreateFoundAnimalUserPreferences.SpeciesId!)
			.Returns(UserPreferences.Species);
		_breedRepositoryMock.GetBreedByIdAsync((int)CreateFoundAnimalUserPreferences.BreedId!).ReturnsNull();

		async Task Result() => await _sut.CreatePreferences(CreateFoundAnimalUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Raça especificada não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_From_Different_Species_Throws_BadRequestException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_speciesRepositoryMock.GetSpeciesByIdAsync((int)CreateFoundAnimalUserPreferences.SpeciesId!)
			.Returns(UserPreferences.Species);
		const int idFromDifferentSpecies = 99;
		Breed breedFromDifferentSpecies = new()
		{
			Species = new Species() { Id = idFromDifferentSpecies }
		};
		_breedRepositoryMock.GetBreedByIdAsync((int)CreateFoundAnimalUserPreferences.BreedId!)
			.Returns(breedFromDifferentSpecies);

		async Task Result() => await _sut.CreatePreferences(CreateFoundAnimalUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Raça não pertence a espécie especificada.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_With_Non_Existent_Colors_Throws_NotFoundException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_speciesRepositoryMock.GetSpeciesByIdAsync((int)CreateFoundAnimalUserPreferences.SpeciesId!)
			.Returns(UserPreferences.Species);
		_breedRepositoryMock.GetBreedByIdAsync((int)CreateFoundAnimalUserPreferences.BreedId!)
			.Returns(UserPreferences.Breed);
		List<Color> emptyColorsLIst = new();
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreateFoundAnimalUserPreferences.ColorIds)
			.Returns(emptyColorsLIst);

		async Task Result() => await _sut.CreatePreferences(CreateFoundAnimalUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_Returns_Created_Preferences()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_speciesRepositoryMock.GetSpeciesByIdAsync((int)CreateFoundAnimalUserPreferences.SpeciesId!)
			.Returns(UserPreferences.Species);
		_breedRepositoryMock.GetBreedByIdAsync((int)CreateFoundAnimalUserPreferences.BreedId!)
			.Returns(UserPreferences.Breed);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreateFoundAnimalUserPreferences.ColorIds)
			.Returns(UserPreferences.Colors.ToList());
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_valueProviderMock.NewGuid().Returns(UserPreferences.Id);

		var createdPreferences = await _sut.CreatePreferences(CreateFoundAnimalUserPreferences, User.Id);

		Assert.Equivalent(ExpectedUserPreferencesResponse, createdPreferences);
	}
}