using System.Collections.Generic;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;

namespace Tests.EntityGenerators;

public static class PetGenerator
{
    public static Pet GeneratePet()
    {
        User owner = UserGenerator.GenerateUser();
        Breed breed = BreedGenerator.GenerateBreed();
        Species species = SpeciesGenerator.GenerateSpecies();

        return new Pet
        {
            Id = Guid.NewGuid(),
            Name = "animal",
            Observations = "observations",
            Owner = owner,
            UserId = owner.Id,
            Breed = breed,
            BreedId = breed.Id,
            Species = species,
            SpeciesId = species.Id,
            Colors = ColorGenerator.GenerateListOfColors()
        };
    }

    public static Pet GeneratePetWithId(Guid id)
    {
        User owner = UserGenerator.GenerateUser();
        Breed breed = BreedGenerator.GenerateBreed();
        Species species = SpeciesGenerator.GenerateSpecies();

        return new Pet
        {
            Id = id,
            Name = "animal",
            Observations = "observations",
            Owner = owner,
            UserId = owner.Id,
            Breed = breed,
            BreedId = breed.Id,
            Species = species,
            SpeciesId = species.Id,
            Colors = ColorGenerator.GenerateListOfColors()
        };
    }

    public static Pet GeneratePetWithoutOwner()
    {
        Breed breed = BreedGenerator.GenerateBreed();
        Species species = SpeciesGenerator.GenerateSpecies();

        return new Pet()
        {
            Id = Guid.NewGuid(),
            Name = "animal",
            Observations = "observations",
            Breed = breed,
            BreedId = breed.Id,
            Species = species,
            SpeciesId = species.Id,
            Colors = ColorGenerator.GenerateListOfColors()
        };
    }

    public static CreatePetRequest GenerateCreatePetRequest()
    {
        return new CreatePetRequest()
        {
            Name = "animal",
            Observations = "observations",
            BreedId = 1,
            SpeciesId = 1,
            ColorIds = new List<int>() { 1 }
        };
    }

    public static EditPetRequest GenerateEditPetRequest()
    {
        return new EditPetRequest()
        {
            Id = Guid.NewGuid(),
            Name = "animal",
            Observations = "observations",
            BreedId = 1,
            SpeciesId = 1,
            ColorIds = new List<int>() { 1 }
        };
    }
    
    public static Pet GeneratePetFromCreatePetRequest(
        CreatePetRequest createPetRequest,
        Guid petId,
        User? owner,
        Breed breed,
        Species species,
        List<Color> colors)
    {
        return new Pet()
        {
            Id = petId,
            Name = createPetRequest.Name,
            Observations = createPetRequest.Observations,
            Owner = owner,
            Breed = breed,
            Species = species,
            Colors = colors
        };
    }
}