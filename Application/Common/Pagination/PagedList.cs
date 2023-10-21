using Microsoft.EntityFrameworkCore;

namespace Application.Common.Pagination;

public class PagedList<T> : List<T>
{
    public const int MaxPageSize = 50;
    public int CurrentPage { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalCount { get; private set; }
    private int _pageSize;

    public int PageSize
    {
        get => _pageSize;
        private set => _pageSize = Math.Min(value, MaxPageSize);
    }

    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        int count = items.Count;

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}