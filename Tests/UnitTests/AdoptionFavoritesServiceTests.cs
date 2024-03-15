using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts;
using Application.Common.Interfaces.Entities.AdoptionFavoriteAlerts.DTOs;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities.UserFavorites;
using Domain.Entities;
using Domain.Entities.Alerts;
using Domain.Entities.Alerts.UserFavorites;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;
using Tests.EntityGenerators.Alerts.UserFavorites;

namespace Tests.UnitTests;

public class AdoptionFavoritesServiceTests
{
	private readonly IAdoptionFavoritesRepository _adoptionFavoritesRepositoryMock;
	private readonly IAdoptionAlertRepository _adoptionAlertRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly AdoptionFavoritesService _sut;

	private static readonly AdoptionFavorite AdoptionFavorite = AdoptionFavoritesGenerator.GenerateAdoptionFavorite();
	private static readonly AdoptionAlert AdoptionAlert = AdoptionAlertGenerator.GenerateAdoptedAdoptionAlert();

	private static readonly AdoptionFavoriteResponse AdoptionFavoriteResponse =
		AdoptionFavoritesGenerator.GenerateResponse();

	private static readonly User User = UserGenerator.GenerateUser();

	private const int Page = 1;
	private const int PageSize = 25;

	public AdoptionFavoritesServiceTests()
	{
		_adoptionFavoritesRepositoryMock = Substitute.For<IAdoptionFavoritesRepository>();
		_adoptionAlertRepositoryMock = Substitute.For<IAdoptionAlertRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		_sut = new AdoptionFavoritesService(
			_adoptionFavoritesRepositoryMock,
			_adoptionAlertRepositoryMock,
			_userRepositoryMock, _valueProviderMock);
	}

	[Fact]
	public async Task Get_Non_Existent_Adoption_Favorite_By_Id_Throws_NotFoundException()
	{
		_adoptionFavoritesRepositoryMock.GetByIdAsync(AdoptionFavorite.Id, User.Id).ReturnsNull();

		async Task Result() => await _sut.GetByIdAsync(AdoptionFavorite.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Não foi possível encontrar o item favoritado.", exception.Message);
	}

	[Fact]
	public async Task Get_Adoption_Favorite_By_Id_Returns_Adoption_Favorite()
	{
		_adoptionFavoritesRepositoryMock.GetByIdAsync(AdoptionFavorite.Id, User.Id).Returns(AdoptionFavorite);

		AdoptionFavoriteResponse returnedAdoptionFavorite = await _sut.GetByIdAsync(AdoptionFavorite.Id, User.Id);

		Assert.Equivalent(AdoptionFavoriteResponse, returnedAdoptionFavorite);
	}

	[Fact]
	public async Task Get_All_Adoption_Alerts_Returns_Paginated_Adoption_Alerts()
	{
		var pagedFavorites = PagedListGenerator.GeneratePagedAdoptionFavorites();
		_adoptionFavoritesRepositoryMock.ListFavoritesAsync(User.Id, Page, PageSize)
			.Returns(pagedFavorites);
		var expectedFavorites = PaginatedEntityGenerator.GeneratePaginatedAdoptionFavoriteResponse();

		var alertsResponse = await _sut.GetAllAdoptionFavorites(User.Id, Page, PageSize);

		Assert.Equivalent(expectedFavorites, alertsResponse);
	}

	[Fact]
	public async Task Toggle_Favorite_Of_Non_Existent_Alert_Throws_NotFoundException()
	{
		_adoptionAlertRepositoryMock.GetByIdAsync(AdoptionAlert.Id).ReturnsNull();

		async Task Result() => await _sut.ToggleFavorite(User.Id, AdoptionAlert.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Toggle_Non_Existent_Favorite_Returns_Created_Entity()
	{
		_adoptionAlertRepositoryMock.GetByIdAsync(AdoptionAlert.Id).Returns(AdoptionAlert);
		_adoptionFavoritesRepositoryMock.GetFavoriteAlertAsync(User.Id, AdoptionAlert.Id).ReturnsNull();
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_valueProviderMock.NewGuid().Returns(AdoptionFavorite.Id);

		AdoptionFavoriteResponse returnedFavorite = await _sut.ToggleFavorite(User.Id, AdoptionAlert.Id);

		Assert.Equivalent(AdoptionFavoriteResponse, returnedFavorite);
	}

	[Fact]
	public async Task Toggle_Existing_Favorite_Returns_Deleted_Favorite()
	{
		_adoptionAlertRepositoryMock.GetByIdAsync(AdoptionAlert.Id).Returns(AdoptionAlert);
		_adoptionFavoritesRepositoryMock.GetFavoriteAlertAsync(User.Id, AdoptionAlert.Id).ReturnsNull();
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_adoptionFavoritesRepositoryMock.GetFavoriteAlertAsync(User.Id, AdoptionAlert.Id).Returns(AdoptionFavorite);

		AdoptionFavoriteResponse returnedFavorite = await _sut.ToggleFavorite(User.Id, AdoptionAlert.Id);

		Assert.Equivalent(AdoptionFavoriteResponse, returnedFavorite);
	}
}