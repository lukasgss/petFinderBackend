using RabbitMQ.Client;

namespace Infrastructure.ExternalServices.RabbitMQ;

public interface IMessagingConnectionEstablisher
{
	IConnection EstablishConnection();
}