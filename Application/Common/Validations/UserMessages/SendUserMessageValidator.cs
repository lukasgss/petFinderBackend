using Application.Common.Interfaces.Entities.UserMessages.DTOs;
using FluentValidation;

namespace Application.Common.Validations.UserMessages;

public class SendUserMessageValidator : AbstractValidator<SendUserMessageRequest>
{
    public SendUserMessageValidator()
    {
        RuleFor(message => message.Content)
            .NotEmpty()
            .WithMessage("Campo de conteúdo é obrigatório.")
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");

        RuleFor(message => message.ReceiverId)
            .NotEmpty()
            .WithMessage("Campo de recebedor é obrigatório.");
    }
}