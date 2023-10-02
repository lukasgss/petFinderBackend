using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Domain.Entities;

namespace Application.Services.Entities;

public class UserMessageService : IUserMessageService
{
    private readonly IUserMessageRepository _userMessageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserMessageService(
        IUserMessageRepository userMessageRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userMessageRepository = userMessageRepository ?? throw new ArgumentNullException(nameof(userMessageRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    public async Task<UserMessageResponse> GetByIdAsync(long messageId, Guid receiverId)
    {
        UserMessage? message = await _userMessageRepository.GetByIdAsync(messageId, receiverId);
        if (message is null)
        {
            throw new NotFoundException("Mensagem com o id especificado não existe.");
        }
        
        return message.ToUserMessageResponse();
    }

    public async Task<PaginatedEntity<UserMessageResponse>> GetMessagesFromSenderAsync(Guid senderId, Guid receiverId, int pageNumber, int pageSize)
    {
        var messages = await _userMessageRepository.GetAllAsync(senderId, receiverId, pageNumber, pageSize);
        await MarkAllMessagesAsReadAsync(senderId, receiverId);

        return messages.ToUserMessageResponsePagedList();
    }

    public async Task<UserMessageResponse> SendAsync(SendUserMessageRequest messageRequest, Guid senderId)
    {
        User? receiver = await _userRepository.GetUserByIdAsync(messageRequest.ReceiverId);
        if (receiver is null)
        {
            throw new NotFoundException("Usuário destinatário não foi encontrado.");
        }

        User? sender = await _userRepository.GetUserByIdAsync(senderId);
        if (sender is null)
        {
            throw new NotFoundException("Usuário remetente não foi encontrado.");
        }

        UserMessage message = new()
        {
            Content = messageRequest.Content,
            TimeStamp = _dateTimeProvider.UtcNow(),
            HasBeenRead = false,
            Sender = sender,
            Receiver = receiver
        };
        
        _userMessageRepository.Add(message);
        await _userMessageRepository.CommitAsync();

        return message.ToUserMessageResponse();
    }
    
    private async Task MarkAllMessagesAsReadAsync(Guid senderId, Guid receiverId)
    {
        await _userMessageRepository.ReadAllAsync(senderId, receiverId);
    }
}