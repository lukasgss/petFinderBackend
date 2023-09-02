using System.Collections.Generic;
using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Entities;
using Constants = Tests.TestUtils.Constants.Constants;

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
            Id = Constants.PetData.Id,
            Name = Constants.PetData.Name,
            Observations= Constants.PetData.Observations,
            Owner = Constants.PetData.User,
            UserId = Constants.UserData.Id,
            Breed = breed,
            BreedId = breed.Id,
            Species = species,
            SpeciesId = species.Id,
            Colors = ColorGenerator.GenerateListOfColors()
        };
    }

    public static Pet GeneratePetWithoutOwner()
    {
        return new Pet()
        {
            Id = Constants.PetData.Id,
            Name = Constants.PetData.Name,
            Observations = Constants.PetData.Observations,
            Owner = null,
            UserId = null,
            Breed = Constants.PetData.Breed,
            BreedId = Constants.PetData.BreedId,
            Species = Constants.PetData.Species,
            SpeciesId = Constants.PetData.SpeciesId,
            Colors = ColorGenerator.GenerateListOfColors()
        };
    }

    public static List<Pet> GenerateListOfPet()
    {
        return new List<Pet>()
        {
            GeneratePet()
        };
    }

    public static CreatePetRequest GenerateCreatePetRequest()
    {
        return new CreatePetRequest()
        {
            Name = Constants.PetData.Name,
            Observations = Constants.PetData.Observations,
            BreedId = Constants.PetData.BreedId,
            SpeciesId = Constants.PetData.SpeciesId,
            ColorIds = Constants.PetData.ColorIds
        };
    }

    public static EditPetRequest GenerateEditPetRequest()
    {
        return new EditPetRequest()
        {
            Id = Constants.PetData.Id,
            Name = Constants.PetData.Name,
            Observations = Constants.PetData.Observations,
            BreedId = Constants.PetData.BreedId,
            SpeciesId = Constants.PetData.SpeciesId,
            ColorIds = Constants.PetData.ColorIds
        };
    }
}