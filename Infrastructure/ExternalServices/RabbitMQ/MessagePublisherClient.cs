using System.Text;
using System.Text.Json;
using Application.Common.Interfaces.ExternalServices.MessagePublisher;
using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.ExternalServices.RabbitMQ;

public class MessagePublisherClient : IMessagePublisherClient
{
	private readonly IMessagingConnectionEstablisher _messagingConnectionEstablisher;
	private readonly RabbitMqData _rabbitMqData;

	public MessagePublisherClient(IMessagingConnectionEstablisher messagingConnectionEstablisher,
		IOptions<RabbitMqData> rabbitMqData)
	{
		_messagingConnectionEstablisher = messagingConnectionEstablisher;
		_rabbitMqData = rabbitMqData.Value;
	}

	public void PublishMessage<T>(T message) where T : class
	{
		using IConnection connection = _messagingConnectionEstablisher.EstablishConnection();
		using IModel channel = connection.CreateModel();

		SetupRouting(channel);

		string json = JsonSerializer.Serialize(message);
		byte[] body = Encoding.UTF8.GetBytes(json);

		channel.BasicPublish(exchange: _rabbitMqData.AlertsExchangeName,
			routingKey: _rabbitMqData.FoundAnimalsRoutingKey,
			body: body);
	}

	private void SetupRouting(IModel channel)
	{
		channel.ExchangeDeclare(_rabbitMqData.AlertsExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
		channel.QueueDeclare(queue: _rabbitMqData.FoundAnimalsQueueName,
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null);
		channel.QueueBind(_rabbitMqData.FoundAnimalsQueueName,
			_rabbitMqData.AlertsExchangeName,
			_rabbitMqData.FoundAnimalsRoutingKey);
	}
}