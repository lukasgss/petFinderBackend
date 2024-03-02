using Application.Common.Interfaces.Converters;

namespace Application.Services.Converters;

public class IdConverterService : IIdConverterService
{
	public string ConvertGuidToShortId(Guid id)
	{
		return FormatId(id);
	}

	public string ConvertGuidToShortId(Guid id, int index)
	{
		string formattedId = FormatId(id);

		// The index is necessary for multiple file uploads, not repeating
		// the same ids for the URL
		return index == 0 ? formattedId : $"{formattedId}{index}";
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

	private static string FormatId(Guid id)
	{
		string base64Guid = Convert.ToBase64String(id.ToByteArray());

		// Replace URL unfriendly characters
		base64Guid = base64Guid.Replace('+', '-').Replace('/', '_');

		// Remove the trailing ==
		return base64Guid.Substring(0, base64Guid.Length - 2);
	}
}