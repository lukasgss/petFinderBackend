namespace Application.Common.Interfaces.Converters;

public interface IIdConverterService
{
	string ConvertGuidToShortId(Guid id);
	string ConvertGuidToShortId(Guid id, int index);
	Guid DecodeShortIdToGuid(string shortId);
}