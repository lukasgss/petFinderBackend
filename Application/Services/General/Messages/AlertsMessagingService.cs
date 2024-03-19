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
			foundAlert.Age,
			foundAlert.Size,
			FoundLocationLatitude = foundAlert.Location.Y,
			FoundLocationLongitude = foundAlert.Location.X,
			SpeciesId = foundAlert.Species.Id,
			BreedId = foundAlert.Breed?.Id,
			ColorIds = foundAlert.Colors.Select(color => color.Id)
		};

		_messagePublisherClient.PublishMessage(data, MessageType.FoundAnimal);
	}

	public void PublishAdoptionAlert(AdoptionAlert adoptionAlert)
	{
		var data = new
		{
			adoptionAlert.Id,
			adoptionAlert.Pet.Gender,
			adoptionAlert.Pet.Age,
			adoptionAlert.Pet.Size,
			FoundLocationLatitude = adoptionAlert.Location.Y,
			FoundLocationLongitude = adoptionAlert.Location.X,
			SpeciesId = adoptionAlert.Pet.Species.Id,
			BreedId = adoptionAlert.Pet.Breed.Id,
			ColorIds = adoptionAlert.Pet.Colors.Select(color => color.Id)
		};

		_messagePublisherClient.PublishMessage(data, MessageType.AdoptionAnimal);
	}
}