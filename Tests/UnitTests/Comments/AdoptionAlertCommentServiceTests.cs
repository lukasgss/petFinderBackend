using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts;
using Application.Common.Interfaces.Entities.Alerts.Comments;
using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
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

public class AdoptionAlertCommentServiceTests
{
	private readonly IAdoptionAlertCommentRepository _adoptionAlertCommentRepositoryMock;
	private readonly IAdoptionAlertRepository _adoptionAlertRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IDateTimeProvider _dateTimeProviderMock;
	private readonly IGuidProvider _guidProviderMock;
	private readonly IAdoptionAlertCommentService _sut;

	private const int Page = 1;
	private const int PageSize = 25;

	private static readonly AdoptionAlertComment AdoptionAlertComment =
		AdoptionAlertCommentGenerator.GenerateAdoptionAlertComment();

	private static readonly AlertCommentResponse ExpectedAlertCommentResponse =
		GenericAlertCommentGenerator.GenerateAdoptionAlertCommentResponse();

	private static readonly AdoptionAlert AdoptionAlert = AdoptionAlertGenerator.GenerateAdoptedAdoptionAlert();

	private static readonly CreateAlertCommentRequest CreateAlertCommentRequest =
		GenericAlertCommentGenerator.GenerateCreateAlertCommentRequest();

	private static readonly User User = UserGenerator.GenerateUser();

	public AdoptionAlertCommentServiceTests()
	{
		_adoptionAlertCommentRepositoryMock = Substitute.For<IAdoptionAlertCommentRepository>();
		_adoptionAlertRepositoryMock = Substitute.For<IAdoptionAlertRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
		_guidProviderMock = Substitute.For<IGuidProvider>();

		_sut = new AdoptionAlertCommentService(
			_adoptionAlertCommentRepositoryMock,
			_adoptionAlertRepositoryMock,
			_userRepositoryMock,
			_dateTimeProviderMock,
			_guidProviderMock);
	}

	[Fact]
	public async Task Get_Non_Existent_Comment_By_Id_Throws_NotFoundException()
	{
		_adoptionAlertCommentRepositoryMock.GetByIdAsync(AdoptionAlertComment.Id).ReturnsNull();

		async Task Result() => await _sut.GetCommentByIdAsync(AdoptionAlertComment.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Comentário com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Get_Comment_By_Id_Returns_Comment()
	{
		_adoptionAlertCommentRepositoryMock.GetByIdAsync(AdoptionAlertComment.Id).Returns(AdoptionAlertComment);

		var commentResponse = await _sut.GetCommentByIdAsync(AdoptionAlertComment.Id);

		Assert.Equivalent(ExpectedAlertCommentResponse, commentResponse);
	}

	[Fact]
	public async Task List_Alert_Comments_Returns_Alert_Comments()
	{
		var pagedComments = PagedListGenerator.GeneratePagedAdoptionAlertComments();
		_adoptionAlertCommentRepositoryMock.GetCommentsByAlertIdAsync(AdoptionAlert.Id, Page, PageSize)
			.Returns(pagedComments);
		var expectedComments = PaginatedEntityGenerator.GeneratePaginatedAdoptionAlertCommentResponse();

		var commentsResponse = await _sut.ListCommentsAsync(AdoptionAlertComment.AdoptionAlert.Id, Page, PageSize);

		Assert.Equivalent(expectedComments, commentsResponse);
	}

	[Fact]
	public async Task Post_Comment_With_Route_Different_Than_Route_Id_Throws_BadRequestException()
	{
		Guid differentRouteId = Guid.NewGuid();
		async Task Result() => await _sut.PostCommentAsync(CreateAlertCommentRequest, User.Id, differentRouteId);


		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
	}

	[Fact]
	public async Task Post_Comment_On_Non_Existent_Alert_Throws_NotFoundException()
	{
		_adoptionAlertRepositoryMock.GetByIdAsync(CreateAlertCommentRequest.AlertId).ReturnsNull();

		async Task Result() =>
			await _sut.PostCommentAsync(CreateAlertCommentRequest, User.Id, CreateAlertCommentRequest.AlertId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Alerta com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Post_Comment_Returns_Posted_Comment()
	{
		_adoptionAlertRepositoryMock.GetByIdAsync(CreateAlertCommentRequest.AlertId).Returns(AdoptionAlert);
		_userRepositoryMock.GetUserByIdAsync(User.Id).Returns(User);
		_guidProviderMock.NewGuid().Returns(AdoptionAlertComment.Id);
		_dateTimeProviderMock.UtcNow().Returns(AdoptionAlertComment.Date);

		var commentResponse =
			await _sut.PostCommentAsync(CreateAlertCommentRequest, User.Id, CreateAlertCommentRequest.AlertId);

		Assert.Equivalent(ExpectedAlertCommentResponse, commentResponse);
	}

	[Fact]
	public async Task Delete_Non_Existent_Comment_Throws_NotFoundException()
	{
		_adoptionAlertCommentRepositoryMock.GetByIdAsync(AdoptionAlertComment.Id).ReturnsNull();

		async Task Result() => await _sut.DeleteCommentAsync(AdoptionAlertComment.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Comentário com o id especificado não existe.", exception.Message);
	}

	[Fact]
	public async Task Delete_Comment_From_Another_User_Throws_UnauthorizedException()
	{
		Guid differentUserId = Guid.NewGuid();
		_adoptionAlertCommentRepositoryMock.GetByIdAsync(AdoptionAlertComment.Id).Returns(AdoptionAlertComment);

		async Task Result() => await _sut.DeleteCommentAsync(AdoptionAlertComment.Id, differentUserId);

		var exception = await Assert.ThrowsAsync<UnauthorizedException>(Result);
		Assert.Equal("Não é possível excluir comentários de outros usuários.", exception.Message);
	}
}