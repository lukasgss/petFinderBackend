using System.Linq;
using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts.UserPreferences;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Application.Common.Interfaces.General.UserPreferences;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using Domain.Entities.Alerts.UserPreferences;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts.UserPreferences;
using Tests.EntityGenerators.Alerts.UserPreferences.Common;

namespace Tests.UnitTests;

public class AdoptionAlertUserPreferencesServiceTests
{
	private readonly IAdoptionUserPreferencesRepository _adoptionUserPreferencesRepositoryMock;
	private readonly IUserPreferencesValidations _userPreferencesValidationsMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IAdoptionAlertUserPreferencesService _sut;

	private static readonly AdoptionUserPreferences UserPreferences =
		AdoptionUserPreferencesGenerator.GenerateAdoptionUserPreferences();

	private static readonly UserPreferencesResponse ExpectedUserPreferencesResponse =
		AdoptionUserPreferencesGenerator.GenerateAdoptionUserPreferences()
			.ToAdoptionUserPreferencesResponse();

	private static readonly CreateAlertsUserPreferences CreateAlertsUserPreferences =
		UserPreferencesGenerator.GenerateCreateFoundAnimalUserPreferences();

	private static readonly User User = UserGenerator.GenerateUser();

	public AdoptionAlertUserPreferencesServiceTests()
	{
		_adoptionUserPreferencesRepositoryMock = Substitute.For<IAdoptionUserPreferencesRepository>();
		_userPreferencesValidationsMock = Substitute.For<IUserPreferencesValidations>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		var loggerMock = Substitute.For<ILogger<AdoptionAlertUserPreferencesService>>();
		_sut = new AdoptionAlertUserPreferencesService(_adoptionUserPreferencesRepositoryMock,
			_userPreferencesValidationsMock, _valueProviderMock, loggerMock);
	}

	[Fact]
	public async Task Get_User_Preferences_Of_User_With_No_Preferences_Throws_NotFoundException()
	{
		_adoptionUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();

		async Task Result() => await _sut.GetUserPreferences(User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Ainda não foram definidas preferências desse tipo de alerta para este usuário.",
			exception.Message);
	}

	[Fact]
	public async Task Get_User_Preferences_Returns_User_Preferences()
	{
		_adoptionUserPreferencesRepositoryMock.GetUserPreferences(User.Id).Returns(UserPreferences);

		UserPreferencesResponse userPreferencesResponse = await _sut.GetUserPreferences(User.Id);

		Assert.Equivalent(ExpectedUserPreferencesResponse, userPreferencesResponse);
	}

	[Fact]
	public async Task Create_User_Preferences_When_Already_Registered_Throws_BadRequestException()
	{
		_adoptionUserPreferencesRepositoryMock.GetUserPreferences(User.Id).Returns(UserPreferences);

		async Task Result() => await _sut.CreatePreferences(CreateAlertsUserPreferences, User.Id);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Usuário já possui preferências cadastradas para esse tipo de alerta.", exception.Message);
	}

	[Fact]
	public async Task Create_User_Preferences_Returns_Created_Preferences()
	{
		_adoptionUserPreferencesRepositoryMock.GetUserPreferences(User.Id).ReturnsNull();
		_userPreferencesValidationsMock.ValidateAndAssignSpeciesAsync(CreateAlertsUserPreferences.SpeciesIds!)
			.Returns(UserPreferences.Species!);
		_userPreferencesValidationsMock
			.ValidateAndAssignBreedAsync(CreateAlertsUserPreferences.BreedIds!, UserPreferences.Species!)
			.Returns(UserPreferences.Breeds!);
		_userPreferencesValidationsMock.ValidateAndAssignColorsAsync(CreateAlertsUserPreferences.ColorIds)
			.Returns(UserPreferences.Colors!.ToList());
		_userPreferencesValidationsMock.AssignUserAsync(User.Id).Returns(User);
		_valueProviderMock.NewGuid().Returns(UserPreferences.Id);

		var createdPreferences = await _sut.CreatePreferences(CreateAlertsUserPreferences, User.Id);

		Assert.Equivalent(ExpectedUserPreferencesResponse, createdPreferences);
	}
}