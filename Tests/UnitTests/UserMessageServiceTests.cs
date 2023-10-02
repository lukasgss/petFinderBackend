using Application.Common.Exceptions;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Common.Pagination;
using Application.Services.Entities;
using Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Tests.EntityGenerators;
using Tests.TestUtils.Constants;

namespace Tests.UnitTests;

public class UserMessageServiceTests
{
    private readonly IUserMessageRepository _userMessageRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IDateTimeProvider _dateTimeProviderMock;

    private readonly IUserMessageService _sut;

    private static readonly UserMessage UserMessage = UserMessageGenerator.GenerateUserMessage();
    private static readonly User User = UserGenerator.GenerateUser();

    private static readonly SendUserMessageRequest UserMessageRequest =
        UserMessageGenerator.GenerateSendUserMessageRequest();
    private static readonly UserMessageResponse UserMessageResponse = UserMessageGenerator.GenerateUserMessageResponse();

    public UserMessageServiceTests()
    {
        _userMessageRepositoryMock = Substitute.For<IUserMessageRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _dateTimeProviderMock = Substitute.For<IDateTimeProvider>();

        _sut = new UserMessageService(_userMessageRepositoryMock, _userRepositoryMock, _dateTimeProviderMock);
    }

    [Fact]
    public async Task Get_Message_By_Non_Existent_Id_Throws_NotFoundException()
    {
        _userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId).ReturnsNull();

        async Task Result() => await _sut.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId);

        var exception = await Assert.ThrowsAsync<NotFoundException>(Result);
        Assert.Equal("Mensagem com o id especificado não existe.", exception.Message);
    }

    [Fact]
    public async Task Get_Message_By_Id_Returns_UserMessageResponse()
    {
        _userMessageRepositoryMock.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId).Returns(UserMessage);

        UserMessageResponse userMessageResponse = await _sut.GetByIdAsync(UserMessage.Id, UserMessage.ReceiverId);
        
        Assert.Equivalent(UserMessageResponse, userMessageResponse);
    }

    [Fact]
    public async Task Get_Messages_From_Sender_Returns_Messages()
    {
        PagedList<UserMessage> pagedUserMessages = PagedListGenerator.GeneratePagedUserMessages();
        _userMessageRepositoryMock.GetAllAsync(UserMessage.SenderId, UserMessage.ReceiverId, 1, 50).Returns(pagedUserMessages);
        var expectedUserMessageResponse = PaginatedEntityGenerator.GeneratePaginatedUserMessageResponse();

        var userMessageResponse = 
            await _sut.GetMessagesFromSenderAsync(UserMessage.SenderId, UserMessage.ReceiverId, 1, 50);
        
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
        _dateTimeProviderMock.UtcNow().Returns(Constants.UserMessageData.TimeStamp);

        UserMessageResponse userMessageResponse =
            await _sut.SendAsync(UserMessageRequest, Constants.UserMessageData.SenderId);
        userMessageResponse.Id = Constants.UserMessageData.Id;
        
        Assert.Equivalent(UserMessageResponse, userMessageResponse);
    }
}