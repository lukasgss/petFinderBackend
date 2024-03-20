using System.Collections.Generic;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts;
using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Application.Common.Interfaces.Entities.AnimalSpecies;
using Application.Common.Interfaces.Entities.Breeds;
using Application.Common.Interfaces.Entities.Colors;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.General.Images;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Application.Services.General.Messages;
using Domain.Entities;
using Domain.Entities.Alerts;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class FoundAnimalAlertServiceTests
{
	private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepositoryMock;
	private readonly ISpeciesRepository _speciesRepositoryMock;
	private readonly IBreedRepository _breedRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IColorRepository _colorRepositoryMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IFoundAlertImageSubmissionService _imageSubmissionServiceMock;
	private readonly IAlertsMessagingService _alertsMessagingServiceMock;
	private readonly IFoundAnimalAlertService _sut;

	private static readonly FoundAnimalAlert FoundAnimalAlert = FoundAnimalAlertGenerator.GenerateFoundAnimalAlert();

	private static readonly FoundAnimalAlertFilters FoundAnimalAlertFilters =
		FoundAnimalAlertGenerator.GenerateFoundAnimalAlertFilters();

	private static readonly CreateFoundAnimalAlertRequest CreateFoundAnimalAlertRequest =
		FoundAnimalAlertGenerator.GenerateCreateFoundAnimalAlertRequest();

	private static readonly EditFoundAnimalAlertRequest EditFoundAnimalAlertRequest =
		FoundAnimalAlertGenerator.GenerateEditFoundAnimalAlertRequest();

	private static readonly FoundAnimalAlertResponse FoundAnimalAlertResponse =
		FoundAnimalAlertGenerator.GenerateFoundAnimalAlertResponse();

	private static readonly User User = UserGenerator.GenerateUser();
	private static readonly Species Species = SpeciesGenerator.GenerateSpecies();
	private static readonly List<Color> Colors = ColorGenerator.GenerateListOfColors();
	private static readonly Breed Breed = BreedGenerator.GenerateBreed();

	private const int Page = 1;
	private const int PageSize = 25;

	public FoundAnimalAlertServiceTests()
	{
		_foundAnimalAlertRepositoryMock = Substitute.For<IFoundAnimalAlertRepository>();
		_speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
		_breedRepositoryMock = Substitute.For<IBreedRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_colorRepositoryMock = Substitute.For<IColorRepository>();
		_imageSubmissionServiceMock = Substitute.For<IFoundAlertImageSubmissionService>();
		_alertsMessagingServiceMock = Substitute.For<IAlertsMessagingService>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		var loggerMock = Substitute.For<ILogger<FoundAnimalAlertService>>();

		_sut = new FoundAnimalAlertService(
			_foundAnimalAlertRepositoryMock,
			_speciesRepositoryMock,
			_breedRepositoryMock,
			_userRepositoryMock,
			_colorRepositoryMock,
			_imageSubmissionServiceMock,
			_alertsMessagingServiceMock,
			_valueProviderMock,
			loggerMock);
	}

	[Fact]
	public async Task Get_Non_Existent_Found_Alert_By_Id_Throws_NotFoundException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(FoundAnimalAlert.Id).ReturnsNull();

		async Task Result() => await _sut.GetByIdAsync(FoundAnimalAlert.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Get_Found_Alert_By_Id_Returns_Found_Alert()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(FoundAnimalAlert.Id).Returns(FoundAnimalAlert);

		FoundAnimalAlertResponse foundAlertResponse = await _sut.GetByIdAsync(FoundAnimalAlert.Id);

		Assert.Equivalent(FoundAnimalAlertResponse, foundAlertResponse);
	}

	[Fact]
	public async Task List_Found_Animal_Alerts_With_Page_Less_Than_1_Throws_BadRequestException()
	{
		const int pageSmallerThanOne = 0;
		async Task Result() => await _sut.ListFoundAnimalAlerts(FoundAnimalAlertFilters, pageSmallerThanOne, PageSize);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Insira um número e tamanho de página maior ou igual a 1.", exception.Message);
	}

	[Fact]
	public async Task List_Found_Animal_Alerts_With_Page_Size_Less_Than_1_Throws_BadRequestException()
	{
		const int pageSizeSmallerThanOne = 0;
		async Task Result() => await _sut.ListFoundAnimalAlerts(FoundAnimalAlertFilters, Page, pageSizeSmallerThanOne);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Insira um número e tamanho de página maior ou igual a 1.", exception.Message);
	}

	[Fact]
	public async Task List_Found_Animal_Alerts_Returns_Found_Animal_Alerts()
	{
		var pagedAlerts = PagedListGenerator.GeneratePagedFoundAnimalAlerts();
		var expectedAlerts = PaginatedEntityGenerator.GeneratePaginatedFoundAnimalAlertResponse();
		_foundAnimalAlertRepositoryMock.ListAlertsAsync(FoundAnimalAlertFilters, Page, PageSize)
			.Returns(pagedAlerts);

		var foundAnimalAlerts = await _sut.ListFoundAnimalAlerts(FoundAnimalAlertFilters, Page, PageSize);

		Assert.Equivalent(expectedAlerts, foundAnimalAlerts);
	}

	[Fact]
	public async Task Create_Alert_With_Non_Existing_Species_Throws_NotFoundException()
	{
		_speciesRepositoryMock.GetSpeciesByIdAsync(FoundAnimalAlert.Species.Id).ReturnsNull();

		async Task Result() => await _sut.CreateAsync(CreateFoundAnimalAlertRequest, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Espécie com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Alert_With_Non_Existent_Colors_Throws_NotFoundException()
	{
		_speciesRepositoryMock.GetSpeciesByIdAsync(FoundAnimalAlert.SpeciesId).Returns(Species);
		List<Color> emptyColorsList = new();
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreateFoundAnimalAlertRequest.ColorIds)
			.Returns(emptyColorsList);

		async Task Result() => await _sut.CreateAsync(CreateFoundAnimalAlertRequest, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Alert_With_Non_Existent_Breed_Throws_NotFoundException()
	{
		_speciesRepositoryMock.GetSpeciesByIdAsync(FoundAnimalAlert.SpeciesId).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreateFoundAnimalAlertRequest.ColorIds).Returns(Colors);
		_breedRepositoryMock.GetBreedByIdAsync((int)CreateFoundAnimalAlertRequest.BreedId!).ReturnsNull();

		async Task Result() => await _sut.CreateAsync(CreateFoundAnimalAlertRequest, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Raça com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Alert_Returns_Created_Alert()
	{
		_speciesRepositoryMock.GetSpeciesByIdAsync(FoundAnimalAlert.SpeciesId).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(CreateFoundAnimalAlertRequest.ColorIds).Returns(Colors);
		_breedRepositoryMock.GetBreedByIdAsync((int)CreateFoundAnimalAlertRequest.BreedId!).Returns(Breed);
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_valueProviderMock.NewGuid().Returns(FoundAnimalAlert.Id);
		_imageSubmissionServiceMock
			.UploadImagesAsync(FoundAnimalAlert.Id, CreateFoundAnimalAlertRequest.Images)
			.Returns(Constants.FoundAnimalAlertData.ImageUrls);
		_valueProviderMock.UtcNow().Returns(FoundAnimalAlert.RegistrationDate);

		FoundAnimalAlertResponse createdAlertResponse = await _sut.CreateAsync(CreateFoundAnimalAlertRequest, User.Id);

		Assert.Equivalent(FoundAnimalAlertResponse, createdAlertResponse);
	}

	[Fact]
	public async Task Edit_Alert_With_Different_Route_Id_Than_Specified_Throws_BadRequestException()
	{
		Guid differentRouteId = Guid.NewGuid();

		async Task Result() => await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, differentRouteId);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
	}

	[Fact]
	public async Task Edit_Non_Existent_Alert_Throws_NotFoundException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(EditFoundAnimalAlertRequest.Id).ReturnsNull();

		async Task Result() =>
			await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, EditFoundAnimalAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Alert_From_Other_User_Throws_ForbiddenException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(EditFoundAnimalAlertRequest.Id).Returns(FoundAnimalAlert);
		Guid differentUserId = Guid.NewGuid();

		async Task Result() =>
			await _sut.EditAsync(EditFoundAnimalAlertRequest, differentUserId, EditFoundAnimalAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível editar alertas de outros usuários.", exception.Message);
	}

	[Fact]
	public async Task Edit_Alert_With_Non_Existent_Species_Throws_NotFoundException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(EditFoundAnimalAlertRequest.Id).Returns(FoundAnimalAlert);
		_speciesRepositoryMock.GetSpeciesByIdAsync(EditFoundAnimalAlertRequest.SpeciesId).ReturnsNull();

		async Task Result() =>
			await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, EditFoundAnimalAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Espécie com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Alert_With_Non_Existent_Color_Throws_NotFoundException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(EditFoundAnimalAlertRequest.Id).Returns(FoundAnimalAlert);
		_speciesRepositoryMock.GetSpeciesByIdAsync(EditFoundAnimalAlertRequest.SpeciesId).Returns(Species);
		List<Color> emptyColorsList = new();
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(EditFoundAnimalAlertRequest.ColorIds).Returns(emptyColorsList);

		async Task Result() =>
			await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, EditFoundAnimalAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alguma das cores especificadas não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Alert_With_Non_Existent_Breed_Throws_NotFoundException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(EditFoundAnimalAlertRequest.Id).Returns(FoundAnimalAlert);
		_speciesRepositoryMock.GetSpeciesByIdAsync(EditFoundAnimalAlertRequest.SpeciesId).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(EditFoundAnimalAlertRequest.ColorIds).Returns(Colors);
		_breedRepositoryMock.GetBreedByIdAsync((int)EditFoundAnimalAlertRequest.BreedId!).ReturnsNull();

		async Task Result() =>
			await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, EditFoundAnimalAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Raça com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Alert_Returns_Edited_Alert()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(EditFoundAnimalAlertRequest.Id).Returns(FoundAnimalAlert);
		_speciesRepositoryMock.GetSpeciesByIdAsync(EditFoundAnimalAlertRequest.SpeciesId).Returns(Species);
		_colorRepositoryMock.GetMultipleColorsByIdsAsync(EditFoundAnimalAlertRequest.ColorIds).Returns(Colors);
		_breedRepositoryMock.GetBreedByIdAsync((int)EditFoundAnimalAlertRequest.BreedId!).Returns(Breed);
		_imageSubmissionServiceMock.UpdateImagesAsync(EditFoundAnimalAlertRequest.Id,
				EditFoundAnimalAlertRequest.Images, EditFoundAnimalAlertRequest.Images.Count)
			.Returns(Constants.FoundAnimalAlertData.ImageUrls);

		FoundAnimalAlertResponse foundAnimalAlertResponse =
			await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, EditFoundAnimalAlertRequest.Id);

		Assert.Equivalent(FoundAnimalAlertResponse, foundAnimalAlertResponse);
	}

	[Fact]
	public async Task Delete_Non_Existent_Alert_Throws_NotFoundException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(FoundAnimalAlert.Id).ReturnsNull();

		async Task Result() => await _sut.DeleteAsync(FoundAnimalAlert.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Delete_Alert_From_Another_User_Throws_ForbiddenException()
	{
		_foundAnimalAlertRepositoryMock.GetByIdAsync(FoundAnimalAlert.Id).Returns(FoundAnimalAlert);
		Guid differentUserId = Guid.NewGuid();

		async Task Result() => await _sut.DeleteAsync(FoundAnimalAlert.Id, differentUserId);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível excluir alertas de outros usuários.", exception.Message);
	}
}