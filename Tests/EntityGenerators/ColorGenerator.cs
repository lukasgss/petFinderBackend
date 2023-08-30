using System.Collections.Generic;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class ColorGenerator
{
    public static List<Color> GenerateListOfColors()
    {
        return new List<Color>()
        {
            GenerateColor()
        };
    }

    public static Color GenerateColor()
    {
        return new Color()
        {
            Id = 1,
            Name = "Preto",
            HexCode = "#000",
            Pets = new List<Pet>()
        };
    }
}