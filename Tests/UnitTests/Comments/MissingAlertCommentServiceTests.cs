using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using Application.Common.Interfaces.Entities.Alerts.MissingAlerts;
using Application.Common.Interfaces.Entities.MissingAlertComments;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Services.Entities.Comments;
using Domain.Entities;
using Domain.Entities.Alerts;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.EntityGenerators.Alerts;
using Tests.EntityGenerators.Alerts.Comments;

namespace Tests.UnitTests.Comments;

public class MissingAlertCommentServiceTests
{
	private readonly IMissingAlertCommentRepository _missingAlertCommentRepositoryMock;
	private readonly IMissingAlertRepository _missingAlertRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IGuidProvider _guidProviderMock;
	private readonly IDateTimeProvider _dateTimeProviderMock;
	private readonly IMissingAlertCommentService _sut;

	private const int Page = 1;
	private const int PageSize = 25;

	private static readonly MissingAlertComment MissingAlertComment =
		MissingAlertCommentGenerator.GenerateMissingAlertComment();

	private static readonly AlertCommentResponse ExpectedAlertCommentResponse =
		GenericAlertCommentGenerator.GenerateMissingAlertCommentResponse();

	private static readonly CreateAlertCommentRequest CreateAlertCommentRequest =
		GenericAlertCommentGenerator.GenerateCreateAlertCommentRequest();

	private static readonly MissingAlert MissingAlert = MissingAlertGenerator.GenerateMissingAlert();
	private static readonly User User = UserGenerator.GenerateUser();

	public MissingAlertCommentServiceTests()
	{
		_missingAlertCommentRepositoryMock = Substitute.For<IMissingAlertCommentRepository>();
		_missingAlertRepositoryMock = Substitute.For<IMissingAlertRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_guidProviderMock = Substitute.For<IGuidProvider>();
		_dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

		_sut = new MissingAlertCommentService(
			_missingAlertCommentRepositoryMock,
			_missingAlertRepositoryMock,
			_userRepositoryMock,
			_guidProviderMock,
			_dateTimeProviderMock);
	}

	[Fact]
	public async Task Get_Non_Existent_Comment_By_Id_Throws_NotFoundException()
	{
		_missingAlertCommentRepositoryMock.GetByIdAsync(MissingAlertComment.Id).ReturnsNull();

		async Task Result() => await _sut.GetAlertCommentByIdAsync(MissingAlertComment.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Comentário com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Get_Comment_By_Id_Returns_Comment()
	{
		_missingAlertCommentRepositoryMock.GetByIdAsync(MissingAlertComment.Id).Returns(MissingAlertComment);

		AlertCommentResponse commentResponse = await _sut.GetAlertCommentByIdAsync(MissingAlertComment.Id);

		Assert.Equivalent(ExpectedAlertCommentResponse, commentResponse);
	}

	[Fact]
	public async Task List_Missing_Alert_Comment_Returns_Alert_Comments()
	{
		var pagedComments = PagedListGenerator.GeneratePagedMissingAlertsComments();
		_missingAlertCommentRepositoryMock.GetCommentsByAlertIdAsync(MissingAlertComment.MissingAlertId, Page, PageSize)
			.Returns(pagedComments);
		var expectedAlertComments = PaginatedEntityGenerator.GeneratePaginatedAlertCommentResponse();

		var alertComments =
			await _sut.ListMissingAlertCommentsAsync(MissingAlertComment.MissingAlertId, Page, PageSize);

		Assert.Equivalent(expectedAlertComments, alertComments);
	}

	[Fact]
	public async Task Post_Comment_With_Id_Different_Than_Route_Id_Throws_BadRequestException()
	{
		Guid differentRouteId = Guid.NewGuid();

		async Task Result() => await _sut.PostCommentAsync(CreateAlertCommentRequest,
			MissingAlertComment.UserId, differentRouteId);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
	}

	[Fact]
	public async Task Post_Comment_On_Non_Existent_Alert_Throws_NotFoundException()
	{
		_missingAlertRepositoryMock.GetByIdAsync(CreateAlertCommentRequest.AlertId).ReturnsNull();

		async Task Result() => await _sut.PostCommentAsync(CreateAlertCommentRequest, MissingAlertComment.UserId,
			CreateAlertCommentRequest.AlertId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Post_Comment_Returns_Posted_Comment()
	{
		_missingAlertRepositoryMock.GetByIdAsync(CreateAlertCommentRequest.AlertId).Returns(MissingAlert);
		_userRepositoryMock.GetUserByIdAsync(MissingAlertComment.UserId).Returns(User);
		_guidProviderMock.NewGuid().Returns(MissingAlertComment.Id);
		_dateTimeProviderMock.UtcNow().Returns(MissingAlertComment.Date);

		var alertCommentResponse = await _sut.PostCommentAsync(
			CreateAlertCommentRequest, MissingAlertComment.UserId, CreateAlertCommentRequest.AlertId);

		Assert.Equivalent(ExpectedAlertCommentResponse, alertCommentResponse);
	}

	[Fact]
	public async Task Delete_Non_Existent_Comment_Throws_NotFoundException()
	{
		_missingAlertCommentRepositoryMock.GetByIdAsync(MissingAlertComment.Id).ReturnsNull();

		async Task Result() => await _sut.DeleteCommentAsync(MissingAlertComment.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Comentário com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Delete_Comment_From_Other_User_Throws_UnauthorizedException()
	{
		Guid differentUserId = Guid.NewGuid();
		_missingAlertCommentRepositoryMock.GetByIdAsync(MissingAlertComment.Id).Returns(MissingAlertComment);

		async Task Result() => await _sut.DeleteCommentAsync(MissingAlertComment.Id, differentUserId);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível excluir comentários de outros usuários.", exception.Message);
	}
}