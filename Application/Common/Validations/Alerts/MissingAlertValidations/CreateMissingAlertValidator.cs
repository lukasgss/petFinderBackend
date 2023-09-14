using Application.Common.Interfaces.Entities.Alerts.MissingAlerts.DTOs;
using FluentValidation;

namespace Application.Common.Validations.Alerts.MissingAlertValidations;

public class CreateMissingAlertValidator : AbstractValidator<CreateMissingAlertRequest>
{
    public CreateMissingAlertValidator()
    {
        RuleFor(alert => alert.LastSeenLocationLatitude)
            .NotNull()
            .WithMessage("Campo de latitude é obrigatório.")
            .Must(latitude => latitude >= -90 && latitude <= 90)
            .WithMessage("Campo de latitude deve ser entre -90 e 90.");
        
        RuleFor(alert => alert.LastSeenLocationLongitude)
            .NotNull()
            .WithMessage("Campo de longitude é obrigatório.")
            .Must(longitude => longitude >= -180 && longitude <= 180)
            .WithMessage("Campo de latitude deve ser entre -180 e 180.");

        RuleFor(alert => alert.PetId)
            .NotEmpty()
            .WithMessage("Campo de id do pet é obrigatório.");
    }
}