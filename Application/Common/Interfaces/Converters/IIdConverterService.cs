namespace Application.Common.Interfaces.Converters;

public interface IIdConverterService
{
    string ConvertGuidToShortId(Guid id);
    Guid DecodeShortIdToGuid(string shortId);
}