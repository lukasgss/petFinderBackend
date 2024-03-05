using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using FluentValidation;

namespace Application.Common.Validations.Alerts.FoundAnimalAlertValidations;

public class EditFoundAnimalAlertValidator : AbstractValidator<EditFoundAnimalAlertRequest>
{
	public EditFoundAnimalAlertValidator()
	{
		RuleFor(alert => alert.Id)
			.NotEmpty()
			.WithMessage("Campo de id é obrigatório.");

		RuleFor(alert => alert.Name)
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");

		RuleFor(alert => alert.Description)
			.MaximumLength(500)
			.WithMessage("Máximo de 500 caracteres permitidos.");

		RuleFor(alert => alert.FoundLocationLatitude)
			.NotNull()
			.WithMessage("Campo de latitude é obrigatório.");

		RuleFor(alert => alert.FoundLocationLongitude)
			.NotNull()
			.WithMessage("Campo de longitude é obrigatório.");

		RuleFor(alert => alert.Image)
			.NotEmpty()
			.WithMessage("Campo de imagem é obrigatório.");

		RuleFor(alert => alert.SpeciesId)
			.NotNull()
			.WithMessage("Campo de espécie é obrigatório")
			.GreaterThan(0)
			.WithMessage("Campo recebe apenas valores positivos.");

		RuleFor(pet => pet.ColorIds)
			.NotNull()
			.WithMessage("Campo de cores é obrigatório.");
	}
}