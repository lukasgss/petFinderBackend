using Domain.Entities;

namespace Infrastructure.Persistence.DataSeed;

public static class SeedAges
{
	public static List<Age> Seed()
	{
		return new()
		{
			new Age() { Id = 1, Name = "Filhote" },
			new Age() { Id = 2, Name = "Jovem" },
			new Age() { Id = 3, Name = "Adulto" },
			new Age() { Id = 4, Name = "Sênior" },
		};
	}
}