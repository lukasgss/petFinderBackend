using Application.Common.Interfaces.Entities.Alerts.DTOs;
using FluentValidation;

namespace Application.Common.Validations.Alerts.MissingAlertValidations;

public class EditMissingAlertValidator : AbstractValidator<EditMissingAlertRequest>
{
    public EditMissingAlertValidator()
    {
        RuleFor(alert => alert.Id)
            .NotEmpty()
            .WithMessage("Campo de id é obrigatório.");

        RuleFor(alert => alert.OwnerName)
            .NotEmpty()
            .WithMessage("Campo de nome do dono é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");

        RuleFor(alert => alert.OwnerPhoneNumber)
            .NotEmpty()
            .WithMessage("Campo de telefone do dono é obrigatório.")
            .MaximumLength(30)
            .WithMessage("Máximo de 30 caracteres permitidos.");
        
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

        RuleFor(alert => alert.UserId)
            .NotEmpty()
            .WithMessage("Campo de id do dono é obrigatório.");
    }
}