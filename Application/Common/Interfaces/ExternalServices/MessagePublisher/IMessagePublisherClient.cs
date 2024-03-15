using Application.Services.General.Messages;

namespace Application.Common.Interfaces.ExternalServices.MessagePublisher;

public interface IMessagePublisherClient
{
	void PublishMessage<T>(T message, MessageType messageType) where T : class;
}