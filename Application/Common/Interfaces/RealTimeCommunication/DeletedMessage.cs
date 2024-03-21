namespace Application.Common.Interfaces.RealTimeCommunication;

public class DeletedMessage
{
	public required long Id { get; init; }
	public required Guid SenderId { get; init; }
	public required Guid ReceiverId { get; init; }
}