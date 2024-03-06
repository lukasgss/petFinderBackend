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
	private const int MaximumTimeToEditMessageInMinutes = 7;
	private const int MaximumTimeToDeleteMessageInMinutes = 5;

	private readonly IUserMessageRepository _userMessageRepository;
	private readonly IUserRepository _userRepository;
	private readonly IValueProvider _valueProvider;

	public UserMessageService(
		IUserMessageRepository userMessageRepository,
		IUserRepository userRepository,
		IValueProvider valueProvider)
	{
		_userMessageRepository =
			userMessageRepository ?? throw new ArgumentNullException(nameof(userMessageRepository));
		_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
		_valueProvider = valueProvider ?? throw new ArgumentNullException(nameof(valueProvider));
	}

	public async Task<UserMessageResponse> GetByIdAsync(long messageId, Guid userId)
	{
		UserMessage? message = await _userMessageRepository.GetByIdAsync(messageId, userId);
		if (message is null)
		{
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
			throw new ForbiddenException("Você não possui permissão para ler mensagens de outros usuários.");
		}

		var messages = await _userMessageRepository.GetAllFromUserAsync(senderId, receiverId, pageNumber, pageSize);

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

	public async Task<UserMessageResponse> EditAsync(long messageId, EditUserMessageRequest editUserMessageRequest,
		Guid userId, long routeId)
	{
		if (routeId != editUserMessageRequest.Id)
		{
			throw new BadRequestException("Id da rota não coincide com o id especificado.");
		}

		UserMessage? dbUserMessage = await _userMessageRepository.GetByIdAsync(messageId, userId);
		if (dbUserMessage is null || dbUserMessage.Sender.Id != userId)
		{
			throw new NotFoundException(
				"Mensagem com o id especificado não existe ou você não tem permissão para editá-la.");
		}

		if (_valueProvider.UtcNow().Subtract(dbUserMessage.TimeStampUtc).TotalMinutes >
		    MaximumTimeToEditMessageInMinutes)
		{
			throw new ForbiddenException("Não é possível editar a mensagem, o tempo limite foi expirado.");
		}

		dbUserMessage.Content = editUserMessageRequest.Content;
		dbUserMessage.HasBeenEdited = true;
		await _userMessageRepository.CommitAsync();

		return dbUserMessage.ToUserMessageResponse();
	}

	public async Task DeleteAsync(long messageId, Guid userId)
	{
		UserMessage? dbUserMessage = await _userMessageRepository.GetByIdAsync(messageId, userId);
		if (dbUserMessage is null || dbUserMessage.Sender.Id != userId)
		{
			throw new NotFoundException(
				"Mensagem com o id especificado não existe ou você não tem permissão para excluí-la.");
		}

		if (_valueProvider.UtcNow().Subtract(dbUserMessage.TimeStampUtc).TotalMinutes >
		    MaximumTimeToDeleteMessageInMinutes)
		{
			throw new ForbiddenException(
				"Não é possível excluir a mensagem, o tempo limite foi excedido.");
		}

		dbUserMessage.HasBeenDeleted = true;
		await _userMessageRepository.CommitAsync();
	}

	private async Task MarkAllMessagesAsReadAsync(Guid senderId, Guid receiverId)
	{
		await _userMessageRepository.ReadAllAsync(senderId, receiverId);
	}
}