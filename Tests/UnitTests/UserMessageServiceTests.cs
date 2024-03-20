using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Common.Pagination;
using Application.Services.Entities;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class UserMessageServiceTests
{
	private readonly IUserMessageRepository _userMessageRepositoryMock;
	private readonly IUserRepository _userRepositoryMock;
	private readonly IValueProvider _valueProviderMock;
	private readonly IUserMessageService _sut;

	private static readonly UserMessage UserMessage = UserMessageGenerator.GenerateUserMessage();
	private static readonly UserMessage EditedUserMessage = UserMessageGenerator.GenerateEditedUserMessage();
	private static readonly User User = UserGenerator.GenerateUser();

	private static readonly SendUserMessageRequest UserMessageRequest =
		UserMessageGenerator.GenerateSendUserMessageRequest();

	private static readonly EditUserMessageRequest EditUserMessageRequest =
		UserMessageGenerator.GenerateEditUserMessageRequest();

	private static readonly UserMessageResponse
		UserMessageResponse = UserMessageGenerator.GenerateUserMessageResponse();

	private static readonly UserMessageResponse EditedUserMessageResponse =
		UserMessageGenerator.GenerateEditedUserMessageResponse();


	public UserMessageServiceTests()
	{
		_userMessageRepositoryMock = Substitute.For<IUserMessageRepository>();
		_userRepositoryMock = Substitute.For<IUserRepository>();
		_valueProviderMock = Substitute.For<IValueProvider>();
		var loggerMock = Substitute.For<ILogger<UserMessageService>>();

		_sut = new UserMessageService(_userMessageRepositoryMock, _userRepositoryMock, _valueProviderMock, loggerMock);
	}

	[Fact]
	public async Task Get_Message_By_Non_Existent_Id_Throws_NotFoundException()
	{
		_userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId).ReturnsNull();

		async Task Result() => await _sut.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal(
			"Mensagem com o id especificado não existe ou você não tem permissão para acessá-la.",
			exception.Message);
	}

	[Fact]
	public async Task Get_Message_Without_Being_Sender_Or_Receiver_Throws_NotFoundException()
	{
		Guid userIdNotSenderOrReceiver = Guid.NewGuid();
		_userMessageRepositoryMock
			.GetByIdAsync(Constants.UserMessageData.Id, userIdNotSenderOrReceiver).ReturnsNull();

		async Task Result() =>
			await _sut.GetByIdAsync(Constants.UserMessageData.Id, userIdNotSenderOrReceiver);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal(
			"Mensagem com o id especificado não existe ou você não tem permissão para acessá-la.",
			exception.Message);
	}

	[Fact]
	public async Task Get_Message_By_Id_Returns_UserMessageResponse()
	{
		_userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId).Returns(UserMessage);

		UserMessageResponse userMessageResponse = await _sut.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId);

		Assert.Equivalent(UserMessageResponse, userMessageResponse);
	}

	[Fact]
	public async Task Get_Messages_From_Other_User_Throws_ForbiddenException()
	{
		Guid differentUserId = Guid.NewGuid();

		async Task Result() => await _sut.GetMessagesAsync(
			UserMessage.SenderId, UserMessage.ReceiverId, differentUserId, pageNumber: 1, pageSize: 25);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Você não possui permissão para ler mensagens de outros usuários.", exception.Message);
	}

	[Fact]
	public async Task Get_Messages_From_Sender_Returns_Messages()
	{
		PagedList<UserMessage> pagedUserMessages = PagedListGenerator.GeneratePagedUserMessages();
		_userMessageRepositoryMock.GetAllFromUserAsync(UserMessage.SenderId, UserMessage.ReceiverId, 1, 50)
			.Returns(pagedUserMessages);
		var expectedUserMessageResponse = PaginatedEntityGenerator.GeneratePaginatedUserMessageResponse();

		var userMessageResponse =
			await _sut.GetMessagesAsync(UserMessage.SenderId, UserMessage.ReceiverId, UserMessage.ReceiverId, 1, 50);

		Assert.Equivalent(expectedUserMessageResponse, userMessageResponse);
	}

	[Fact]
	public async Task Send_Message_To_Non_Existent_Receiver_Throws_NotFoundException()
	{
		_userRepositoryMock.GetUserByIdAsync(Constants.UserMessageData.ReceiverId).ReturnsNull();

		async Task Result() => await _sut.SendAsync(UserMessageRequest, Constants.UserMessageData.SenderId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário destinatário não foi encontrado.", exception.Message);
	}

	[Fact]
	public async Task Send_Message_As_Non_Existent_Sender_Throws_NotFoundException()
	{
		_userRepositoryMock.GetUserByIdAsync(UserMessage.ReceiverId).Returns(User);
		_userRepositoryMock.GetUserByIdAsync(UserMessage.SenderId).ReturnsNull();

		async Task Result() => await _sut.SendAsync(UserMessageRequest, UserMessage.SenderId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Usuário remetente não foi encontrado.", exception.Message);
	}

	[Fact]
	public async Task Send_message_Returns_Sent_Message()
	{
		_userRepositoryMock.GetUserByIdAsync(UserMessage.ReceiverId).Returns(UserMessage.Receiver);
		_userRepositoryMock.GetUserByIdAsync(UserMessage.SenderId).Returns(UserMessage.Sender);
		_valueProviderMock.UtcNow().Returns(Constants.UserMessageData.TimeStamp);

		var userMessageResponse =
			await _sut.SendAsync(UserMessageRequest, Constants.UserMessageData.SenderId);
		userMessageResponse.Id = Constants.UserMessageData.Id;

		Assert.Equivalent(UserMessageResponse, userMessageResponse);
	}

	[Fact]
	public async Task
		Edit_Message_With_Route_Different_Than_Specified_On_Request_Throws_BadRequestException()
	{
		const long differentRouteId = 999;

		async Task Result() => await _sut.EditAsync(
			EditUserMessageRequest.Id, EditUserMessageRequest, User.Id, differentRouteId);

		var exception = await Assert.ThrowsAsync<BadRequestException>(Result);
		Assert.Equal("Id da rota não coincide com o id especificado.", exception.Message);
	}

	[Fact]
	public async Task Edit_Non_Existent_Message_Throws_NotFoundException()
	{
		_userMessageRepositoryMock.GetByIdAsync(EditUserMessageRequest.Id, Constants.UserData.Id)
			.ReturnsNull();

		async Task Result() => await _sut.EditAsync(
			EditUserMessageRequest.Id, EditUserMessageRequest, User.Id, EditUserMessageRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Mensagem com o id especificado não existe ou você não tem permissão para editá-la.",
			exception.Message);
	}

	[Fact]
	public async Task Edit_Message_Without_Being_Its_Sender_Throws_NotFoundException()
	{
		Guid differentUserId = Guid.NewGuid();
		_userMessageRepositoryMock
			.GetByIdAsync(EditUserMessageRequest.Id, User.Id)
			.Returns(UserMessage);

		async Task Result() => await _sut.EditAsync(
			EditUserMessageRequest.Id, EditUserMessageRequest, differentUserId, EditUserMessageRequest.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal("Mensagem com o id especificado não existe ou você não tem permissão para editá-la.",
			exception.Message);
	}

	[Fact]
	public async Task Edit_Message_After_Maximum_Edit_Time_Has_Passed_Throws_ForbiddenException()
	{
		const int maximumTimeToEditMessageInMinutes = 7;
		UserMessage userMessage = UserMessageGenerator.GenerateUserMessage();
		DateTime currentDateTime = DateTime.Now;
		userMessage.TimeStampUtc = currentDateTime;
		_userMessageRepositoryMock.GetByIdAsync(EditUserMessageRequest.Id, userMessage.Sender.Id).Returns(userMessage);
		_valueProviderMock.UtcNow().Returns(currentDateTime.AddMinutes(maximumTimeToEditMessageInMinutes + 1));

		async Task Result() => await _sut.EditAsync(
			UserMessage.Id, EditUserMessageRequest, userMessage.Sender.Id, EditUserMessageRequest.Id);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível editar a mensagem, o tempo limite foi expirado.",
			exception.Message);
	}

	[Fact]
	public async Task Edit_Message_Returns_Edited_Message()
	{
		_userMessageRepositoryMock.GetByIdAsync(EditUserMessageRequest.Id, UserMessage.Sender.Id)
			.Returns(EditedUserMessage);
		_valueProviderMock.UtcNow().Returns(UserMessage.TimeStampUtc);

		var userMessageResponse = await _sut.EditAsync(
			EditUserMessageRequest.Id, EditUserMessageRequest, UserMessage.Sender.Id, EditUserMessageRequest.Id);

		Assert.Equivalent(EditedUserMessageResponse, userMessageResponse);
	}

	[Fact]
	public async Task Delete_Non_Existent_Message_Throws_NotFoundException()
	{
		_userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, User.Id).ReturnsNull();

		async Task Result() => await _sut.DeleteAsync(UserMessage.Id, User.Id);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal(
			"Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.",
			exception.Message);
	}

	[Fact]
	public async Task Delete_Message_Without_Being_Sender_Throws_NotFoundException()
	{
		Guid differentUSerId = Guid.NewGuid();
		_userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, User.Id).ReturnsNull();

		async Task Result() => await _sut.DeleteAsync(UserMessage.Id, differentUSerId);

		var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
		Assert.Equal(
			"Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.",
			exception.Message);
	}

	[Fact]
	public async Task
		Delete_Message_After_Maximum_Delete_Time_Has_Passed_Throws_ForbiddenException()
	{
		const int maximumTimeToEditMessageInMinutes = 5;
		UserMessage userMessage = UserMessageGenerator.GenerateUserMessage();
		DateTime currentDateTime = DateTime.Now;
		userMessage.TimeStampUtc = currentDateTime;
		_userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, userMessage.Sender.Id).Returns(userMessage);
		_valueProviderMock.UtcNow()
			.Returns(currentDateTime.AddMinutes(maximumTimeToEditMessageInMinutes + 1));

		async Task Result() => await _sut.DeleteAsync(userMessage.Id, userMessage.Sender.Id);

		var exception = await Assert.ThrowsAsync<ForbiddenException>(Result);
		Assert.Equal("Não é possível excluir a mensagem, o tempo limite foi excedido.",
			exception.Message);
	}
}