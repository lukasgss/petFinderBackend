using Application.Common.Exceptions;
using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Paginated;
using Application.Common.Interfaces.Entities.UserMessages;
using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using Application.Common.Interfaces.Entities.Users;
using Application.Common.Interfaces.Providers;
using Application.Common.Interfaces.RealTimeCommunication;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.Entities;

public class UserMessageService : IUserMessageService
{
	private const int MaximumTimeToEditMessageInMinutes = 7;
	private const int MaximumTimeToDeleteMessageInMinutes = 5;

	private readonly IUserMessageRepository _userMessageRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;
	private readonly IRealTimeChatClient _realTimeChatClient;
	private readonly ILogger<UserMessageService> _logger;

	public UserMessageService(
		IUserMessageRepository userMessageRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider,
		IRealTimeChatClient realTimeChatClient,
		ILogger<UserMessageService> logger)
	{
		_userMessageRepository =
			userMessageRepository ?? throw new ArgumentNullException(nameof(userMessageRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
		_realTimeChatClient = realTimeChatClient ?? throw new ArgumentNullException(nameof(realTimeChatClient));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<UserMessageResponse> GetByIdAsync(long messageId, Guid userId)
	{
		UserMessage? message = await _userMessageRepository.GetByIdAsync(messageId, userId);
		if (message is null)
		{
			_logger.LogInformation(
				"Mensagem {MessageId} não existe ou usuário {UserId} não possui permissão para acessá-la",
				messageId, userId);
			throw new NotFoundException(
				"Mensagem com o id especificado não existe ou você não tem permissão para acessá-la.");
		}

		return message.ToUserMessageResponse();
	}

	public async Task<PaginatedEntity<UserMessageResponse>> GetMessagesAsync(
		Guid senderId, Guid receiverId, Guid currentUserId, int pageNumber, int pageSize)
	{
		if (currentUserId != senderId && currentUserId != receiverId)
		{
			_logger.LogInformation(
				"Usuário {UserId} não possui permissão para ler mensagens entre {ReceiverId} e {SenderId}",
				currentUserId, receiverId, senderId);
			throw new ForbiddenException("Você não possui permissão para ler mensagens de outros usuários.");
		}

		var messages = await _userMessageRepository.GetAllFromUserAsync(senderId, receiverId, pageNumber, pageSize);

		if (currentUserId == receiverId)
		{
			await MarkAllMessagesAsReadAsync(senderId, receiverId);
		}

		return messages.ToUserMessageResponsePagedList();
	}

	public async Task<UserMessageResponse> SendAsync(Guid? senderId, Guid? receiverId, string messageContent)
	{
		ValidateUserIds(senderId, receiverId);

		User? receiver = await _userRepository.GetUserByIdAsync(receiverId!.Value);
		if (receiver is null)
		{
			_logger.LogInformation("Destinatário {ReceiverId} não existe", receiverId);
			throw new NotFoundException("Usuário destinatário não foi encontrado.");
		}

		User? sender = await _userRepository.GetUserByIdAsync(senderId!.Value);
		if (sender is null)
		{
			_logger.LogInformation("Remetente {SenderId} não existe", senderId);
			throw new NotFoundException("Usuário remetente não foi encontrado.");
		}

		UserMessage message = new()
		{
			Content = messageContent,
			TimeStampUtc = _valueProvider.UtcNow(),
			HasBeenRead = false,
			HasBeenEdited = false,
			Sender = sender,
			Receiver = receiver
		};

		_userMessageRepository.Add(message);
		await _userMessageRepository.CommitAsync();

		return message.ToUserMessageResponse();
	}

	public async Task<UserMessageResponse> EditAsync(long messageId, EditUserMessageRequest editRequest,
		Guid userId, long routeId)
	{
		if (routeId != editRequest.Id)
		{
			_logger.LogInformation("Id {RouteId} não coincide com {MessageId}", routeId, editRequest.Id);
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		UserMessage? dbUserMessage = await _userMessageRepository.GetByIdAsync(messageId, userId);
		if (dbUserMessage is null || dbUserMessage.Sender.Id != userId)
		{
			_logger.LogInformation(
				"Mensagem {MessageId} não existe ou usuário {UserId} não possui permissão para editá-la",
				messageId, userId);
			throw new NotFoundException(
				"Mensagem com o id especificado não existe ou você não tem permissão para editá-la.");
		}

		if (_valueProvider.UtcNow().Subtract(dbUserMessage.TimeStampUtc).TotalMinutes >
		    MaximumTimeToEditMessageInMinutes)
		{
			_logger.LogInformation("Não é possível editar mensagem {MessageId}, cadastrada em {MessageTimeStamp}",
				messageId, dbUserMessage.TimeStampUtc);
			throw new ForbiddenException("Não é possível editar a mensagem, o tempo limite foi expirado.");
		}

		await SendEditedMessageRealTime(userId, dbUserMessage.ReceiverId, messageId, editRequest.Content);

		dbUserMessage.Content = editRequest.Content;
		dbUserMessage.HasBeenEdited = true;
		await _userMessageRepository.CommitAsync();

		return dbUserMessage.ToUserMessageResponse();
	}

	public async Task DeleteAsync(long messageId, Guid userId)
	{
		UserMessage? dbUserMessage = await _userMessageRepository.GetByIdAsync(messageId, userId);
		if (dbUserMessage is null || dbUserMessage.Sender.Id != userId)
		{
			_logger.LogInformation(
				"Mensagem {MessageId} não existe ou usuário {UserId} não tem permissão para excluí-la",
				messageId, userId);
			throw new NotFoundException(
				"Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.");
		}

		if (_valueProvider.UtcNow().Subtract(dbUserMessage.TimeStampUtc).TotalMinutes >
		    MaximumTimeToDeleteMessageInMinutes)
		{
			_logger.LogInformation("Não é possível excluir mensagem {MessageId}, cadastrada em {MessageTimeStamp}",
				messageId, dbUserMessage.TimeStampUtc);
			throw new ForbiddenException(
				"Não é possível excluir a mensagem, o tempo limite foi excedido.");
		}

		await DeleteMessageRealTime(userId, dbUserMessage.ReceiverId, messageId);

		dbUserMessage.HasBeenDeleted = true;
		await _userMessageRepository.CommitAsync();
	}

	private async Task MarkAllMessagesAsReadAsync(Guid senderId, Guid receiverId)
	{
		await _userMessageRepository.ReadAllAsync(senderId, receiverId);
	}

	private void ValidateUserIds(Guid? senderId, Guid? receiverId)
	{
		if (senderId is null)
		{
			_logger.LogInformation("SenderId {SenderId} é nulo", senderId);
			throw new BadRequestException("Id do remetente inválido.");
		}

		if (receiverId is null)
		{
			_logger.LogInformation("ReceiverId {ReceiverId} é nulo", receiverId);
			throw new BadRequestException("Id do recebedor inválido.");
		}
	}

	private async Task SendEditedMessageRealTime(Guid senderId, Guid receiverId, long messageId, string messageContent)
	{
		EditedMessage editMessage = new()
		{
			Id = messageId,
			Content = messageContent,
			SenderId = senderId,
			ReceiverId = receiverId
		};
		await _realTimeChatClient.EditMessage(senderId: senderId, receiverId: receiverId, editMessage);
	}

	private async Task DeleteMessageRealTime(Guid senderId, Guid receiverId, long messageId)
	{
		DeletedMessage deletedMessage = new()
		{
			Id = messageId,
			SenderId = senderId,
			ReceiverId = receiverId
		};
		await _realTimeChatClient.DeleteMessage(senderId: senderId, receiverId: receiverId, deletedMessage);
	}
}