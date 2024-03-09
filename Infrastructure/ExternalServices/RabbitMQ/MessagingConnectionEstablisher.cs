using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.ExternalServices.RabbitMQ;

public class MessagingConnectionEstablisher : IMessagingConnectionEstablisher
{
	private readonly RabbitMqData _rabbitMqData;
	private readonly IConnection? _connection = null;

	public MessagingConnectionEstablisher(IOptions<RabbitMqData> rabbitMqData)
	{
		_rabbitMqData = rabbitMqData.Value;
	}

	public IConnection EstablishConnection()
	{
		if (_connection is not null)
		{
			return _connection;
		}

		ConnectionFactory factory = new()
		{
			HostName = _rabbitMqData.HostName,
			UserName = _rabbitMqData.Username,
			Password = _rabbitMqData.Password,
			Port = _rabbitMqData.Port,
		};

		return factory.CreateConnection();
	}
}