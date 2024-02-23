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
using Domain.Entities;
using Domain.Entities.Alerts;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;

namespace Tests.UnitTests;

public class FoundAnimalAlertServiceTests
{
	private readonly IFoundAnimalAlertRepository _foundAnimalAlertRepositoryMock;
	private readonly ISpeciesRepository _speciesRepositoryMock;
	private readonly IBreedRepository _breedRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IColorRepository _colorRepositoryMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IImageSubmissionService _imageSubmissionServiceMock;
	private readonly IFoundAnimalAlertService _sut;

	private static readonly FoundAnimalAlert FoundAnimalAlert = FoundAnimalAlertGenerator.GenerateFoundAnimalAlert();

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

	public FoundAnimalAlertServiceTests()
	{
		_foundAnimalAlertRepositoryMock = Substitute.For<IFoundAnimalAlertRepository>();
		_speciesRepositoryMock = Substitute.For<ISpeciesRepository>();
		_breedRepositoryMock = Substitute.For<IBreedRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_colorRepositoryMock = Substitute.For<IColorRepository>();
		_imageSubmissionServiceMock = Substitute.For<IImageSubmissionService>();
		_valueProviderMock = Substitute.For<IValueProvider>();

		_sut = new FoundAnimalAlertService(
			_foundAnimalAlertRepositoryMock,
			_speciesRepositoryMock,
			_breedRepositoryMock,
			_userRepositoryMock,
			_colorRepositoryMock,
			_imageSubmissionServiceMock,
			_valueProviderMock);
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
		_imageSubmissionServiceMock.UploadFoundAlertImageAsync(FoundAnimalAlert.Id, CreateFoundAnimalAlertRequest.Image)
			.Returns(FoundAnimalAlert.Image);
		_valueProviderMock.UtcNow().Returns(FoundAnimalAlert.RegistrationDate);

		var createdAlertResponse = await _sut.CreateAsync(CreateFoundAnimalAlertRequest, User.Id);

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
		_imageSubmissionServiceMock.UploadFoundAlertImageAsync(EditFoundAnimalAlertRequest.Id,
			EditFoundAnimalAlertRequest.Image).Returns(FoundAnimalAlert.Image);

		FoundAnimalAlertResponse foundAnimalAlertResponse =
			await _sut.EditAsync(EditFoundAnimalAlertRequest, User.Id, EditFoundAnimalAlertRequest.Id);

		Assert.Equivalent(FoundAnimalAlertResponse, foundAnimalAlertResponse);
	}
}