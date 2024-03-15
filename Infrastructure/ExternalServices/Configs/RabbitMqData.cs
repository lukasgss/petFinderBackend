namespace Infrastructure.ExternalServices.Configs;

public class RabbitMqData
{
	public required string HostName { get; init; }
	public required string Username { get; init; }
	public required string Password { get; init; }
	public required int Port { get; init; }
	public required string AlertsExchangeName { get; init; }
	public required string FoundAnimalsRoutingKey { get; init; }
	public required string FoundAnimalsQueueName { get; init; }
	public required string AdoptionAnimalsRoutingKey { get; init; }
	public required string AdoptionAnimalsQueueName { get; init; }
}