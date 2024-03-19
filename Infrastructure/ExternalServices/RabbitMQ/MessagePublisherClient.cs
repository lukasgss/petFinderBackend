using System.Text;
using System.Text.Json;
using Application.Common.Interfaces.ExternalServices.MessagePublisher;
using Application.Services.General.Messages;
using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Infrastructure.ExternalServices.RabbitMQ;

public class MessagePublisherClient : IMessagePublisherClient
{
	private readonly IMessagingConnectionEstablisher _messagingConnectionEstablisher;
	private readonly RabbitMqData _rabbitMqData;
	private readonly ILogger<MessagePublisherClient> _logger;

	private readonly Dictionary<MessageType, string> _routingKeyMap;

	public MessagePublisherClient(
		IMessagingConnectionEstablisher messagingConnectionEstablisher,
		IOptions<RabbitMqData> rabbitMqData,
		ILogger<MessagePublisherClient> logger)
	{
		_messagingConnectionEstablisher = messagingConnectionEstablisher ??
		                                  throw new ArgumentNullException(nameof(messagingConnectionEstablisher));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_rabbitMqData = rabbitMqData.Value ?? throw new ArgumentNullException(nameof(rabbitMqData));

		_routingKeyMap = new Dictionary<MessageType, string>()
		{
			{ MessageType.FoundAnimal, _rabbitMqData.FoundAnimalsRoutingKey },
			{ MessageType.AdoptionAnimal, _rabbitMqData.AdoptionAnimalsRoutingKey },
		};
	}

	public void PublishMessage<T>(T message, MessageType messageType) where T : class
	{
		try
		{
			IConnection connection = _messagingConnectionEstablisher.EstablishConnection();
			using IModel channel = connection.CreateModel();

			SetupRouting(channel);
			string routingKey = _routingKeyMap[messageType];

			string json = JsonSerializer.Serialize(message);
			byte[] body = Encoding.UTF8.GetBytes(json);

			channel.BasicPublish(exchange: _rabbitMqData.AlertsExchangeName,
				routingKey: routingKey,
				body: body);
		}
		catch (BrokerUnreachableException ex)
		{
			_logger.LogError("Não foi possível conectar ao serviço de mensageria. {Exception}", ex);
		}
		catch (Exception ex)
		{
			_logger.LogError("Não foi possível inserir à fila. {Exception}", ex);
		}
	}

	private void SetupRouting(IModel channel)
	{
		channel.ExchangeDeclare(_rabbitMqData.AlertsExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
		SetupFoundAnimalsQueue(channel);
		SetupAdoptionAnimalsQueue(channel);
	}

	private void SetupFoundAnimalsQueue(IModel channel)
	{
		channel.QueueDeclare(queue: _rabbitMqData.FoundAnimalsQueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null);
		channel.QueueBind(_rabbitMqData.FoundAnimalsQueueName,
			_rabbitMqData.AlertsExchangeName,
			_rabbitMqData.FoundAnimalsRoutingKey);
	}

	private void SetupAdoptionAnimalsQueue(IModel channel)
	{
		channel.QueueDeclare(queue: _rabbitMqData.AdoptionAnimalsQueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null);
		channel.QueueBind(_rabbitMqData.AdoptionAnimalsQueueName,
			_rabbitMqData.AlertsExchangeName,
			_rabbitMqData.AdoptionAnimalsRoutingKey);
	}
}