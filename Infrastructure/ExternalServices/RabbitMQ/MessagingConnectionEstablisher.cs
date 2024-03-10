using Infrastructure.ExternalServices.Configs;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.ExternalServices.RabbitMQ;

public class MessagingConnectionEstablisher : IMessagingConnectionEstablisher
{
	private readonly RabbitMqData _rabbitMqData;
	private IConnection? _connection;

	public MessagingConnectionEstablisher(IOptions<RabbitMqData> rabbitMqData)
	{
		_rabbitMqData = rabbitMqData.Value ?? throw new ArgumentNullException(nameof(rabbitMqData));
	}

	public IConnection EstablishConnection()
	{
		if (_connection is not null && _connection.IsOpen)
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
		_connection = factory.CreateConnection();

		return _connection;
	}
}