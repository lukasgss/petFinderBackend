namespace Application.Common.Interfaces.Messaging;

public class MessagingSettings
{
    public string From { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Host { get; init; } = null!;
    public int Port { get; init; }
}