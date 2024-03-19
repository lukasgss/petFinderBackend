using Application.Common.Interfaces.Entities.Sizes;
using Application.Common.Interfaces.FrontendDropdownData;
using Domain.Enums;

namespace Application.Services.Entities;

public class SizeService : ISizeService
{
	private readonly List<DropdownDataResponse<int>> _sizes = new();

	public SizeService()
	{
		foreach (Size size in Enum.GetValues<Size>())
		{
			_sizes.Add(new DropdownDataResponse<int>()
			{
				Text = Enum.GetName(typeof(Size), size)!,
				Value = (int)size
			});
		}
	}

	public IEnumerable<DropdownDataResponse<int>> GetSizes()
	{
		return _sizes;
	}
}