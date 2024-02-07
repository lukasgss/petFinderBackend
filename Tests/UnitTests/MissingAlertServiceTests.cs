using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping.Alerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using Application.Common.Interfaces.Entities.Pets;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities;
using Domain.Entities;
using Domain.Entities.Alerts;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class MissingAlertServiceTests
{
	private readonly IMissingAlertRepository _missingAlertRepositoryMock;
	private readonly IPetRepository _petRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IGuidProvider _guidProviderMock;
	private readonly IDateTimeProvider _dateTimeProviderMock;
	private readonly IMissingAlertService _sut;

	private const int Page = 1;
	private const int PageSize = 25;

	private static readonly MissingAlert MissingAlert = MissingAlertGenerator.GenerateMissingAlert();
	private static readonly MissingAlertResponse ExpectedMissingAlert = MissingAlert.ToMissingAlertResponse();

	private static readonly CreateMissingAlertRequest CreateMissingAlertRequest =
		MissingAlertGenerator.GenerateCreateMissingAlert();

	private static readonly Pet Pet = PetGenerator.GeneratePet();

	private static readonly EditMissingAlertRequest EditMissingAlertRequest =
		MissingAlertGenerator.GenerateEditMissingAlertRequest();

	private static readonly User User = UserGenerator.GenerateUser();

	private static readonly MissingAlertFilters MissingAlertFilters =
		MissingAlertGenerator.GenerateMissingAlertFilters();

	public MissingAlertServiceTests()
	{
		_missingAlertRepositoryMock = Substitute.For<IMissingAlertRepository>();
		_petRepositoryMock = Substitute.For<IPetRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_guidProviderMock = Substitute.For<IGuidProvider>();
		_dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

		_sut = new MissingAlertService(
			_missingAlertRepositoryMock,
			_petRepositoryMock,
			_userRepositoryMock,
			_guidProviderMock,
			_dateTimeProviderMock);
	}

	[Fact]
	public async Task Get_Non_Existent_Missing_Alert_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(MissingAlert.Id).ReturnsNull();

		async Task Result() => await _sut.GetByIdAsync(MissingAlert.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task List_Missing_Alerts_With_Page_Less_Than_1_Throws_BadRequestException()
	{
		const int invalidPageNumber = -1;

		async Task Result() => await _sut.ListMissingAlerts(MissingAlertFilters, invalidPageNumber, 20);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Insira um número e tamanho de página maior ou igual a 1.", exception.Message);
	}

	[Fact]
	public async Task List_Missing_Alerts_Without_Geo_Filters_Returns_Unfiltered_Alerts()
	{
		var pagedAlerts = PagedListGenerator.GeneratePagedMissingAlerts();
		_missingAlertRepositoryMock.ListMissingAlerts(MissingAlertFilters, Page, PageSize)
			.Returns(pagedAlerts);
		var expectedAlerts = PaginatedEntityGenerator.GeneratePaginatedMissingAlertResponse();

		var alertsResponse = await _sut.ListMissingAlerts(MissingAlertFilters, Page, PageSize);

		Assert.Equivalent(expectedAlerts, alertsResponse);
	}

	[Fact]
	public async Task Get_Missing_Alert_By_Id_Returns_Missing_Alert()
	{
		_missingAlertRepositoryMock.GetByIdAsync(MissingAlert.Id).Returns(MissingAlert);

		MissingAlertResponse missingAlertResponse = await _sut.GetByIdAsync(MissingAlert.Id);

		Assert.Equivalent(ExpectedMissingAlert, missingAlertResponse);
	}

	[Fact]
	public async Task Create_Missing_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
	{
		_petRepositoryMock.GetPetByIdAsync(CreateMissingAlertRequest.PetId).ReturnsNull();

		async Task Result() => await _sut.CreateAsync(CreateMissingAlertRequest, Constants.UserData.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Animal com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Create_Missing_Alert_For_Other_Users_Throws_ForbiddenException()
	{
		_petRepositoryMock.GetPetByIdAsync(CreateMissingAlertRequest.PetId).Returns(Pet);
		Guid otherUserId = Guid.NewGuid();
		_userRepositoryMock.GetUserByIdAsync(otherUserId).Returns(User);

		async Task Result() => await _sut.CreateAsync(CreateMissingAlertRequest, userId: otherUserId);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível criar alertas para outros usuários.", exception.Message);
	}

	[Fact]
	public async Task Create_Missing_Alert_Returns_Missing_Alert()
	{
		_petRepositoryMock.GetPetByIdAsync(CreateMissingAlertRequest.PetId).Returns(Pet);
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_dateTimeProviderMock.UtcNow().Returns(ExpectedMissingAlert.RegistrationDate);
		_guidProviderMock.NewGuid().Returns(ExpectedMissingAlert.Id);

		MissingAlertResponse missingAlertResponse =
			await _sut.CreateAsync(CreateMissingAlertRequest, userId: User.Id);

		Assert.Equivalent(ExpectedMissingAlert, missingAlertResponse);
	}

	[Fact]
	public async Task Edit_Alert_With_Route_Id_Different_Than_Specified_On_Request_Throws_BadRequestException()
	{
		async Task Result() => await _sut.EditAsync(
			editMissingAlertRequest: EditMissingAlertRequest,
			userId: Constants.UserData.Id,
			routeId: Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
	}

	[Fact]
	public async Task Edit_Non_Existent_Missing_Alert_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(EditMissingAlertRequest.Id).ReturnsNull();

		async Task Result() => await _sut.EditAsync(
			editMissingAlertRequest: EditMissingAlertRequest,
			userId: User.Id,
			routeId: EditMissingAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Missing_Alert_With_Non_Existent_Pet_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(EditMissingAlertRequest.Id).Returns(MissingAlert);
		_petRepositoryMock.GetPetByIdAsync(EditMissingAlertRequest.PetId).ReturnsNull();

		async Task Result() => await _sut.EditAsync(
			editMissingAlertRequest: EditMissingAlertRequest,
			userId: User.Id,
			routeId: EditMissingAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Animal com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Not_Owned_Missing_Alerts_Throws_ForbiddenException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(EditMissingAlertRequest.Id).Returns(MissingAlert);
		_petRepositoryMock.GetPetByIdAsync(EditMissingAlertRequest.PetId).Returns(Pet);

		async Task Result() => await _sut.EditAsync(
			editMissingAlertRequest: EditMissingAlertRequest,
			userId: Guid.NewGuid(),
			routeId: EditMissingAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível editar alertas de outros usuários.", exception.Message);
	}

	[Fact]
	public async Task Edit_Missing_Alert_With_Non_Existent_User_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(EditMissingAlertRequest.Id).Returns(MissingAlert);
		_petRepositoryMock.GetPetByIdAsync(EditMissingAlertRequest.PetId).Returns(Pet);
		_userRepositoryMock.GetUserByIdAsync(User.Id).ReturnsNull();

		async Task Result() => await _sut.EditAsync(
			editMissingAlertRequest: EditMissingAlertRequest,
			userId: User.Id,
			routeId: EditMissingAlertRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Edit_Missing_Alert_Returns_Edited_Missing_Alert()
	{
		_missingAlertRepositoryMock.GetByIdAsync(EditMissingAlertRequest.Id).Returns(MissingAlert);
		_petRepositoryMock.GetPetByIdAsync(EditMissingAlertRequest.PetId).Returns(Pet);
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);

		MissingAlertResponse missingAlertResponse = await _sut.EditAsync(
			editMissingAlertRequest: EditMissingAlertRequest,
			userId: User.Id,
			routeId: EditMissingAlertRequest.Id);

		Assert.Equivalent(ExpectedMissingAlert, missingAlertResponse);
	}

	[Fact]
	public async Task Delete_Non_Existent_Missing_Alert_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(Constants.MissingAlertData.Id).ReturnsNull();

		async Task Result() => await _sut.DeleteAsync(Constants.MissingAlertData.Id, Constants.UserData.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Delete_Not_Owned_Missing_Alerts_Throws_ForbiddenException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(Constants.MissingAlertData.Id).Returns(MissingAlert);

		async Task Result() => await _sut.DeleteAsync(MissingAlert.Id, userId: Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível excluir alertas de outros usuários.", exception.Message);
	}

	[Fact]
	public async Task Mark_Non_Existent_Missing_Alert_As_Resolved_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(Constants.MissingAlertData.Id).ReturnsNull();

		async Task Result() => await _sut.ToggleFoundStatusAsync(Constants.MissingAlertData.Id, Constants.UserData.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Mark_Missing_Alert_As_Resolved_Not_Being_The_Owner_Throws_ForbiddenException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(MissingAlert.Id).Returns(MissingAlert);

		async Task Result() => await _sut.ToggleFoundStatusAsync(MissingAlert.Id, Guid.NewGuid());

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível marcar alertas de outros usuários como encontrado.", exception.Message);
	}

	[Fact]
	public async Task Mark_Missing_Alert_As_Resolved_Returns_Missing_Alert()
	{
		_missingAlertRepositoryMock.GetByIdAsync(MissingAlert.Id).Returns(MissingAlert);
		MissingAlertResponse expectedMissingAlert = MissingAlertGenerator.GenerateRecoveredMissingAlertResponse();
		_dateTimeProviderMock.DateOnlyNow().Returns(Constants.MissingAlertData.RecoveryDate);

		MissingAlertResponse missingAlertResponse =
			await _sut.ToggleFoundStatusAsync(MissingAlert.Id, MissingAlert.User.Id);

		Assert.Equivalent(expectedMissingAlert, missingAlertResponse);
	}
}