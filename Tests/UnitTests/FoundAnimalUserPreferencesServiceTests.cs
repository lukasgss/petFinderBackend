using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.FoundAnimalAlerts;
using Application.Common.Interfaces.General.UserPreferences;
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
	private readonly IUserPreferencesValidations _userPreferencesValidationsMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IFoundAnimalUserPreferencesService _sut;

	private static readonly FoundAnimalUserPreferences UserPreferences =
		FoundAnimalUserPreferencesGenerator.GenerateFoundAnimalUserPreferences();

	private static readonly UserPreferencesResponse ExpectedUserPreferencesResponse =
		FoundAnimalUserPreferencesGenerator.GenerateFoundAnimalUserPreferences()
			.ToFoundAnimalUserPreferencesResponse();

	private static readonly CreateAlertsUserPreferences CreateAlertsUserPreferences =
		FoundAnimalUserPreferencesGenerator.GenerateCreateFoundAnimalUserPreferences();

	private static readonly User User = UserGenerator.GenerateUser();

	public FoundAnimalUserPreferencesServiceTests()
	{
		_foundAnimalUserPreferencesRepositoryMock = Substitute.For<IFoundAnimalUserPreferencesRepository>();
		_userPreferencesValidationsMock = Substitute.For<IUserPreferencesValidations>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		_sut = new FoundAnimalUserPreferencesService(
			_foundAnimalUserPreferencesRepositoryMock,
			_userPreferencesValidationsMock,
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

		UserPreferencesResponse userPreferencesResponse = await _sut.GetUserPreferences(User.Id);

		Assert.Equivalent(ExpectedUserPreferencesResponse, userPreferencesResponse);
	}

	[Fact]
	public async Task Create_User_Preferences_When_Already_Registered_Throws_BadRequestException()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).Returns(UserPreferences);

		async Task Result() => await _sut.CreatePreferences(CreateAlertsUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Usuário já possui preferências cadastradas.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_Returns_Created_Preferences()
	{
		_foundAnimalUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_userPreferencesValidationsMock.ValidateAndAssignSpeciesAsync((int)CreateAlertsUserPreferences.SpeciesId!)
			.Returns(UserPreferences.Species);
		_userPreferencesValidationsMock
			.ValidateAndAssignBreedAsync((int)CreateAlertsUserPreferences.BreedId!, UserPreferences.SpeciesId)
			.Returns(UserPreferences.Breed);
		_userPreferencesValidationsMock.ValidateAndAssignColorsAsync(CreateAlertsUserPreferences.ColorIds)
			.Returns(UserPreferences.Colors.ToList());
		_userPreferencesValidationsMock.AssignUserAsync(User.Id).Returns(User);
		_valueProviderMock.NewGuid().Returns(UserPreferences.Id);

		var createdPreferences = await _sut.CreatePreferences(CreateAlertsUserPreferences, User.Id);

		Assert.Equivalent(ExpectedUserPreferencesResponse, createdPreferences);
	}
}