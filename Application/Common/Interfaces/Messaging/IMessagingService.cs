namespace Application.Common.Interfaces.Messaging;

public interface IMessagingService
{
    Task SendAccountConfirmationMessageAsync(string userEmail, string confirmationLink);
}