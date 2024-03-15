using Domain.Entities.Alerts;

namespace Application.Services.General.Messages;

public interface IAlertsMessagingService
{
	void PublishFoundAlert(FoundAnimalAlert foundAlert);
	void PublishAdoptionAlert(AdoptionAlert adoptionAlert);
}