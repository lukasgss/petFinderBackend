using Application.Common.Interfaces.Entities.Alerts.FoundAnimalAlerts.DTOs;
using Domain.Enums;
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

		RuleFor(alert => alert.Images)
			.NotEmpty()
			.WithMessage("Campo de imagens é obrigatório.");

		RuleFor(pet => pet.Age)
			.NotNull()
			.WithMessage("Campo de idade é obrigatório.")
			.Must(age => Enum.IsDefined(typeof(Age), age))
			.WithMessage("Valor inválido como idade.");

		RuleFor(pet => pet.Size)
			.NotNull()
			.WithMessage("Campo de porte é obrigatório.")
			.Must(size => Enum.IsDefined(typeof(Size), size))
			.WithMessage("Valor inválido como porte.");

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