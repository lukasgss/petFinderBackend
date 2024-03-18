using Domain.Entities;

namespace Tests.EntityGenerators;

public static class AgeGenerator
{
	public static Age GenerateAge()
	{
		return new Age()
		{
			Id = 1,
			Name = "Senior"
		};
	}
}