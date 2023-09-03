using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.DTOs;
using Domain.Entities.Alerts;
using Constants = Tests.TestUtils.Constants.Constants;

namespace Tests.EntityGenerators.Alerts;

public static class MissingAlertGenerator
{
    public static MissingAlert GenerateMissingAlert()
    {
        return new MissingAlert()
        {
            Id = Constants.MissingAlertData.Id,
            OwnerName = Constants.MissingAlertData.OwnerName,
            OwnerPhoneNumber = Constants.MissingAlertData.OwnerPhoneNumber,
            RegistrationDate = Constants.MissingAlertData.RegistrationDate,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            PetHasBeenRecovered = Constants.MissingAlertData.PetHasBeenRecovered,
            Pet = Constants.MissingAlertData.Pet,
            User = Constants.MissingAlertData.User
        };
    }

    public static MissingAlertResponse GenerateMissingAlertResponse()
    {
        return new MissingAlertResponse()
        {
            Id = Constants.MissingAlertData.Id,
            OwnerName = Constants.MissingAlertData.OwnerName,
            OwnerPhoneNumber = Constants.MissingAlertData.OwnerPhoneNumber,
            RegistrationDate = Constants.MissingAlertData.RegistrationDate,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            PetHasBeenRecovered = Constants.MissingAlertData.PetHasBeenRecovered,
            Pet = Constants.MissingAlertData.Pet.ConvertToPetResponseNoOwner(
                ColorGenerator.GenerateListOfColors().ConvertToListOfColorResponse(),
                BreedGenerator.GenerateBreed().ConvertToBreedResponse()),
            Owner = Constants.MissingAlertData.User?.ConvertToOwnerResponse()
        };
    }
    
    public static MissingAlertResponse GenerateMissingAlertResponseWithoutOwner()
    {
        return new MissingAlertResponse()
        {
            Id = Constants.MissingAlertData.Id,
            OwnerName = Constants.MissingAlertData.OwnerName,
            OwnerPhoneNumber = Constants.MissingAlertData.OwnerPhoneNumber,
            RegistrationDate = Constants.MissingAlertData.RegistrationDate,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            PetHasBeenRecovered = Constants.MissingAlertData.PetHasBeenRecovered,
            Pet = Constants.MissingAlertData.Pet.ConvertToPetResponseNoOwner(
                ColorGenerator.GenerateListOfColors().ConvertToListOfColorResponse(),
                BreedGenerator.GenerateBreed().ConvertToBreedResponse()),
            Owner = null
        };
    }

    public static CreateMissingAlertRequest GenerateCreateMissingAlert()
    {
        return new CreateMissingAlertRequest()
        {
            OwnerName = Constants.MissingAlertData.OwnerName,
            OwnerPhoneNumber = Constants.MissingAlertData.OwnerPhoneNumber,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            UserId = Constants.UserData.Id,
            PetId = Constants.PetData.Id
        };
    }

    public static CreateMissingAlertRequest GenerateCreateMissingAlertWithoutOwner()
    {
        return new CreateMissingAlertRequest()
        {
            OwnerName = Constants.MissingAlertData.OwnerName,
            OwnerPhoneNumber = Constants.MissingAlertData.OwnerPhoneNumber,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            UserId = null,
            PetId = Constants.PetData.Id
        };
    }

    public static EditMissingAlertRequest GenerateEditMissingAlertRequest()
    {
        return new EditMissingAlertRequest()
        {
            OwnerName = Constants.UserData.FullName,
            OwnerPhoneNumber = Constants.UserData.PhoneNumber,
            LastSeenLocationLatitude = Constants.MissingAlertData.LastSeenLocationLatitude,
            LastSeenLocationLongitude = Constants.MissingAlertData.LastSeenLocationLongitude,
            UserId = Constants.MissingAlertData.UserId,
            PetId = Constants.MissingAlertData.PetId
        };
    }
}