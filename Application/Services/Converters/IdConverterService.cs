using Application.Common.Interfaces.Converters;

namespace Application.Services.Converters;

public class IdConverterService : IIdConverterService
{
    public string ConvertGuidToShortId(Guid id)
    {
        string base64Guid = Convert.ToBase64String(id.ToByteArray());
        
        // Replace URL unfriendly characters
        base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

        // Remove the trailing ==
        return base64Guid.Substring(0, base64Guid.Length - 2);
    }
}