using Application.Common.Interfaces.Entities.Colors.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class ColorMappings
{
    public static ColorResponse ConvertToColorResponse(this Color color)
    {
        return new ColorResponse()
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        };
    }

    public static IEnumerable<ColorResponse> ConvertToListOfColorResponse(this IEnumerable<Color> colors)
    {
        List<ColorResponse> colorResponses = new();

        foreach (Color color in colors)
        {
            colorResponses.Add(new ColorResponse()
            {
                Id = color.Id,
                Name = color.Name,
                HexCode = color.HexCode
            });
        }

        return colorResponses;
    }

}
