using Application.Common.Interfaces.Entities.Alerts.AdoptionAlerts.DTOs;
using FluentValidation;

namespace Application.Common.Validations.Alerts.AdoptionAlertValidations;

public class EditAdoptionAlertValidator : AbstractValidator<EditAdoptionAlertRequest>
{
    public EditAdoptionAlertValidator()
    {
        RuleFor(alert => alert.Id)
            .NotEmpty()
            .WithMessage("Campo de id é obrigatório.");

        RuleFor(alert => alert.OnlyForScreenedProperties)
            .NotNull()
            .WithMessage("Campo de apenas imóveis telados é obrigatório.");

        RuleFor(alert => alert.LocationLatitude)
            .NotNull()
            .WithMessage("Campo de latitude é obrigatório.")
            .Must(latitude => latitude is >= -90 and <= 90)
            .WithMessage("Campo de latitude deve ser entre -90 e 90.");

        RuleFor(alert => alert.LocationLongitude)
            .NotNull()
            .WithMessage("Campo de longitude é obrigatório.")
            .Must(longitude => longitude is >= -180 and <= 180)
            .WithMessage("Campo de latitude deve ser entre -180 e 180.");

        RuleFor(alert => alert.Description)
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");

        RuleFor(alert => alert.PetId)
            .NotEmpty()
            .WithMessage("Campo do pet é obrigatório.");
    }
}