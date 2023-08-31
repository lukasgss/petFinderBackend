namespace Application.Common.Interfaces.FrontendDropdownData;

public class DropdownDataResponse<T> where T : struct
{
    public string Text { get; init; } = null!;
    public T Value { get; init; }
}