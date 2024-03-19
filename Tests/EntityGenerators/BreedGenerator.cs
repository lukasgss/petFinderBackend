using System.Collections.Generic;
using Domain.Entities;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators;

public static class BreedGenerator
{
	public static Breed GenerateBreed()
	{
		Species species = SpeciesGenerator.GenerateSpecies();

		return new Breed()
		{
			Id = Constants.BreedData.SpeciesId,
			Name = Constants.BreedData.Name,
			Species = species,
			SpeciesId = species.Id
		};
	}

	public static List<Breed> GenerateListOfBreeds()
	{
		return new List<Breed>()
		{
			GenerateBreed()
		};
	}
}