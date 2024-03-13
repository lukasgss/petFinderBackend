using Application.Common.Interfaces.ExternalServices.MessagePublisher;
using Domain.Entities.Alerts;

namespace Application.Services.General.Messages;

public class AlertsMessagingService : IAlertsMessagingService
{
	private readonly IMessagePublisherClient _messagePublisherClient;

	public AlertsMessagingService(IMessagePublisherClient messagePublisherClient)
	{
		_messagePublisherClient =
			messagePublisherClient ?? throw new ArgumentNullException(nameof(messagePublisherClient));
	}

	public void PublishFoundAlert(FoundAnimalAlert foundAlert)
	{
		var data = new
		{
			foundAlert.Id,
			foundAlert.Gender,
			FoundLocationLatitude = foundAlert.Location.Y,
			FoundLocationLongitude = foundAlert.Location.X,
			SpeciesId = foundAlert.Species.Id,
			BreedId = foundAlert.Breed?.Id,
			ColorIds = foundAlert.Colors.Select(color => color.Id)
		};

		_messagePublisherClient.PublishMessage(data);
	}
}