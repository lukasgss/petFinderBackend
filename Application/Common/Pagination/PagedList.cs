using Microsoft.EntityFrameworkCore;

namespace Application.Common.Pagination;

public class PagedList<T> : List<T>
{
	public const int MaxPageSize = 50;
	public int CurrentPage { get; private set; }
	public int CurrentPageCount { get; private set; }
	public int TotalPages { get; private set; }
	private int _pageSize;

	public int PageSize
	{
		get => _pageSize;
		private set => _pageSize = Math.Min(value, MaxPageSize);
	}

	public PagedList(IReadOnlyCollection<T> items, int currentPageCount, int pageNumber, int pageSize)
	{
		CurrentPage = pageNumber;

		if (items.Count == 0)
		{
			CurrentPageCount = 0;
			PageSize = 0;
			TotalPages = (int)Math.Ceiling(currentPageCount / (double)MaxPageSize);
		}
		else
		{
			CurrentPageCount = currentPageCount;
			PageSize = pageSize;
			TotalPages = (int)Math.Ceiling(currentPageCount / (double)pageSize);
			AddRange(items);
		}
	}

	public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
	{
		if (pageSize > MaxPageSize)
		{
			pageSize = MaxPageSize;
		}

		int count = await source.CountAsync();
		List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

		return new PagedList<T>(items, count, pageNumber, pageSize);
	}
}