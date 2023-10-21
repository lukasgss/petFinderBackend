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

    public async Task<UserMessageResponse> GetByIdAsync(long messageId, Guid userId)
    {
        UserMessage? message = await _userMessageRepository.GetByIdAsync(messageId, userId);
        if (message is null)
        {
            throw new NotFoundException("Mensagem com o id especificado não existe ou você não tem permissão para acessá-la.");

        }
        
        return message.ToUserMessageResponse();
    }

    public async Task<PaginatedEntity<UserMessageResponse>> GetMessagesAsync(
        Guid senderId, Guid receiverId, Guid currentUserId, int pageNumber, int pageSize)
    {
        if (currentUserId != senderId && currentUserId != receiverId)
        {
            throw new NotFoundException("Mensagem com o id especificado não existe ou você não tem permissão para acessá-la.");
        }

        var messages = await _userMessageRepository.GetAllFromUserAsync(
            senderId, receiverId, pageNumber, pageSize);
        
        if (currentUserId == receiverId)
        {
            await MarkAllMessagesAsReadAsync(senderId, receiverId);
        }

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