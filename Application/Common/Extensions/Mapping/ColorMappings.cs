using Application.Common.Interfaces.Entities.Colors.DTOs;
using Domain.Entities;

namespace Application.Common.Extensions.Mapping;

public static class ColorMappings
{
    public static ColorResponse ToColorResponse(this Color color)
    {
        return new ColorResponse()
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        };
    }

    public static IEnumerable<ColorResponse> ToListOfColorResponse(this IEnumerable<Color> colors)
    {
        return colors.Select(color => new ColorResponse()
        {
            Id = color.Id,
            Name = color.Name,
            HexCode = color.HexCode
        }).ToList();
    }
}
