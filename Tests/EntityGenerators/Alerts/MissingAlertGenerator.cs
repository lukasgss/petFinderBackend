using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.DTOs;
using Application.Common.Interfaces.Entities.Users.DTOs;
using Domain.Entities;
using Domain.Entities.Alerts;

namespace Tests.EntityGenerators.Alerts;

public static class MissingAlertGenerator
{
    public static MissingAlert GenerateMissingAlert()
    {
        Pet pet = PetGenerator.GeneratePet();
        User user = UserGenerator.GenerateUser();

        return new MissingAlert()
        {
            Id = Guid.NewGuid(),
            OwnerName = "Text",
            OwnerPhoneNumber = "(11) 11111-1111",
            RegistrationDate = new DateTime(2020, 1, 1),
            LastSeenLocationLatitude = 90,
            LastSeenLocationLongitude = 90,
            PetHasBeenRecovered = false,
            Pet = pet,
            User = user
        };
    }

    public static MissingAlertResponse GenerateMissingAlertResponseFromCreateRequest(
        CreateMissingAlertRequest createMissingAlertRequest)
    {
        return new MissingAlertResponse()
        {
            Id = Guid.NewGuid(),
            OwnerName = createMissingAlertRequest.OwnerName,
            OwnerPhoneNumber = createMissingAlertRequest.OwnerPhoneNumber,
            RegistrationDate = new DateTime(2020, 1, 1),
            LastSeenLocationLatitude = createMissingAlertRequest.LastSeenLocationLatitude,
            LastSeenLocationLongitude = createMissingAlertRequest.LastSeenLocationLongitude,
            PetHasBeenRecovered = false,
            Pet = PetGenerator.GeneratePet().ConvertToPetResponseNoOwner(
                ColorGenerator.GenerateListOfColors().ConvertToListOfColorResponse(),
                BreedGenerator.GenerateBreed().ConvertToBreedResponse()),
            Owner = createMissingAlertRequest.UserId is not null
                ? UserGenerator.GenerateUser().ConvertToOwnerResponse()
                : null
        };
    }

    public static CreateMissingAlertRequest GenerateCreateMissingAlert()
    {
        return new CreateMissingAlertRequest()
        {
            OwnerName = "Text",
            OwnerPhoneNumber = "(11) 11111-1111",
            LastSeenLocationLatitude = 90,
            LastSeenLocationLongitude = 90,
            UserId = Guid.NewGuid(),
            PetId = Guid.NewGuid()
        };
    }

    public static CreateMissingAlertRequest GenerateCreateMissingAlertWithoutOwner()
    {
        return new CreateMissingAlertRequest()
        {
            OwnerName = "Text",
            OwnerPhoneNumber = "(11) 11111-1111",
            LastSeenLocationLatitude = 90,
            LastSeenLocationLongitude = 90,
            UserId = null,
            PetId = Guid.NewGuid()
        };
    }
}