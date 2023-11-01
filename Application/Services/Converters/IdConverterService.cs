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

    public Guid DecodeShortIdToGuid(string shortId)
    {
        // Add the trailing == back
        string base64String = $"{shortId}==";

        // Replace the previously replaced characters back
        base64String = base64String.Replace('-', '+').Replace("_", "/");
        byte[] base64Bytes = Convert.FromBase64String(base64String);

        return new Guid(base64Bytes);
    }
}