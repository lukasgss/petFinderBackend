using Application.Common.Interfaces.Entities.Alerts.UserPreferences.DTOs;
using Domain.Enums;
using FluentValidation;

namespace Application.Common.Validations.Alerts.UserPreferences;

public class CreateAlertsUserPreferencesValidator : AbstractValidator<CreateAlertsUserPreferences>
{
	public CreateAlertsUserPreferencesValidator()
	{
		RuleFor(alert => alert.FoundLocationLatitude)
			.NotNull()
			.When(alert => alert.FoundLocationLongitude != null || alert.RadiusDistanceInKm != null)
			.WithMessage("Caso seja especificada a localização, especifique a latitude.");

		RuleFor(alert => alert.FoundLocationLongitude)
			.NotNull()
			.When(alert => alert.FoundLocationLatitude != null || alert.RadiusDistanceInKm != null)
			.WithMessage("Caso seja especificada a localização, especifique a longitude.");

		RuleFor(alert => alert.RadiusDistanceInKm)
			.NotNull()
			.When(alert => alert.FoundLocationLatitude != null || alert.FoundLocationLongitude != null)
			.WithMessage("Caso seja especificada a localização, especifique o raio de distância em kilômetros.");

		RuleFor(pet => pet.Genders)
			.Must(genders => genders == null || genders.All(gender => Enum.IsDefined(typeof(Gender), gender)))
			.WithMessage("Valor inválido como gênero.");

		RuleFor(pet => pet.Sizes)
			.Must(sizes => sizes == null || sizes.All(size => Enum.IsDefined(typeof(Size), size)))
			.WithMessage("Valor inválido como porte.");

		RuleFor(pet => pet.Ages)
			.Must(ages => ages == null || ages.All(age => Enum.IsDefined(typeof(Age), age)))
			.WithMessage("Valor inválido como idade.");
	}
}