namespace Infrastructure.ExternalServices.Configs;

public class RabbitMqData
{
	public required string HostName { get; set; }
	public required string Username { get; set; }
	public required string Password { get; set; }
	public required int Port { get; set; }
	public required string AlertsExchangeName { get; set; }
	public required string FoundAnimalsRoutingKey { get; set; }
	public required string FoundAnimalsQueueName { get; set; }
	public required int PrefetchCount { get; set; }
}