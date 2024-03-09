namespace Application.Common.Interfaces.ExternalServices.MessagePublisher;

public interface IMessagePublisherClient
{
	void PublishMessage<T>(T message) where T : class;
}