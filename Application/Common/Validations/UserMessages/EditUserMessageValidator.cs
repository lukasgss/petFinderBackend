using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using FluentValidation;

namespace Application.Common.Validations.UserMessages;

public class EditUserMessageValidator : AbstractValidator<EditUserMessageRequest>
{
    public EditUserMessageValidator()
    {
        RuleFor(message => message.Id)
            .NotNull()
            .WithMessage("Campo de id é obrigatório.");

        RuleFor(message => message.Content)
            .NotEmpty()
            .WithMessage("Campo de conteúdo é obrigatório.")
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");
    }
}