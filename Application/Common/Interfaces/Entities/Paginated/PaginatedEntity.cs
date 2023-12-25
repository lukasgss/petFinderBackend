namespace Application.Common.Interfaces.Entities.Paginated;

public class PaginatedEntity<T> where T : class
{
    public List<T> Data { get; init; } = default!;
    public int CurrentPageCount { get; init; }
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
}