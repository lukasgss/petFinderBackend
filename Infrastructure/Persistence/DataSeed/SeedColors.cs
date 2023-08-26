using Domain.Entities;

namespace Infrastructure.Persistence.DataSeed;

public static class SeedColors
{
    public static List<Color> Seed()
    {
        return new()
        {
            new Color() { Id = 1, Name = "Branco", HexCode = "#FFFFFF" },
            new Color() { Id = 2, Name = "Preto", HexCode = "#181818" },
            new Color() { Id = 3, Name = "Marrom", HexCode = "#35281E" },
        };
    }
}