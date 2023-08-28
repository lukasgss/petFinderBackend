namespace Application.Common.Interfaces.Entities.Users.DTOs;

public class OwnerResponse
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
}