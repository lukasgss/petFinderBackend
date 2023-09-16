using Application.Common.Extensions.Mapping;
using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using Domain.Entities.Alerts;
using Tests.TestUtils.Constants;

namespace Tests.EntityGenerators.Alerts;

public static class AdoptionAlertGenerator
{
    public static AdoptionAlert GenerateAdoptedAdoptionAlert()
    {
        return new AdoptionAlert()
        {
            Id = Constants.AdoptionAlertData.Id,
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = Constants.AdoptionAlertData.AdoptedAdoptionDate,
            Pet = Constants.AdoptionAlertData.Pet,
            PetId = Constants.AdoptionAlertData.PetId,
            User = Constants.AdoptionAlertData.User,
            UserId = Constants.AdoptionAlertData.UserId
        };
    }
    
    public static AdoptionAlert GenerateNonAdoptedAdoptionAlert()
    {
        return new AdoptionAlert()
        {
            Id = Constants.AdoptionAlertData.Id,
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = null,
            Pet = Constants.AdoptionAlertData.Pet,
            PetId = Constants.AdoptionAlertData.PetId,
            User = Constants.AdoptionAlertData.User,
            UserId = Constants.AdoptionAlertData.UserId
        };
    }

    public static AdoptionAlert GenerateAdoptionAlertWithRandomOwner()
    {
        return new AdoptionAlert()
        {
            Id = Constants.AdoptionAlertData.Id,
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = null,
            Pet = Constants.AdoptionAlertData.Pet,
            PetId = Constants.AdoptionAlertData.PetId,
            User = UserGenerator.GenerateUserWithRandomId(),
            UserId = UserGenerator.GenerateUserWithRandomId().Id
        };
    }

    public static CreateAdoptionAlertRequest GenerateCreateAdoptionAlertRequest()
    {
        return new CreateAdoptionAlertRequest()
        {
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            PetId = Constants.AdoptionAlertData.PetId,
        };
    }

    public static EditAdoptionAlertRequest GenerateEditAdoptionAlertRequest()
    {
        return new EditAdoptionAlertRequest()
        {
            Id = Constants.AdoptionAlertData.Id,
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            PetId = Constants.AdoptionAlertData.PetId,
        };
    }
    
    public static AdoptionAlertResponse GenerateAdoptedAdoptionAlertResponse()
    {
        return new AdoptionAlertResponse()
        {
            Id = Constants.AdoptionAlertData.Id,
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = Constants.AdoptionAlertData.AdoptedAdoptionDate,
            Pet = Constants.AdoptionAlertData.Pet.ToPetResponseNoOwner(
                Constants.PetData.Colors,
                Constants.PetData.Breed),
            Owner = Constants.AdoptionAlertData.User.ToUserDataResponse(),
        };
    }

    public static AdoptionAlertResponse GenerateNonAdoptedAdoptionAlertResponse()
    {
        return new AdoptionAlertResponse()
        {
            Id = Constants.AdoptionAlertData.Id,
            OnlyForScreenedProperties = Constants.AdoptionAlertData.OnlyForScreenedProperties,
            LocationLatitude = Constants.AdoptionAlertData.LocationLatitude,
            LocationLongitude = Constants.AdoptionAlertData.LocationLongitude,
            Description = Constants.AdoptionAlertData.Description,
            RegistrationDate = Constants.AdoptionAlertData.RegistrationDate,
            AdoptionDate = null,
            Pet = Constants.AdoptionAlertData.Pet.ToPetResponseNoOwner(
                Constants.PetData.Colors,
                Constants.PetData.Breed),
            Owner = Constants.AdoptionAlertData.User.ToUserDataResponse(),
        };
    }
}