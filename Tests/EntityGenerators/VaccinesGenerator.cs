using System.Collections.Generic;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class VaccinesGenerator
{
	public static List<Vaccine> GenerateListOfVaccines()
	{
		return new List<Vaccine>(1)
		{
			new()
			{
				Id = 1,
				Name = "V8",
				Species = new List<Species>()
				{
					new() { Id = 1 }
				}
			},
		};
	}
}