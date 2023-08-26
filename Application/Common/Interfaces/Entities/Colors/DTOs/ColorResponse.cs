namespace Application.Common.Interfaces.Entities.Colors.DTOs;

public class ColorResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string HexCode { get; set; } = null!;
}