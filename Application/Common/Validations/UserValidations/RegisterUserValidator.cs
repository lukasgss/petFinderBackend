using Application.Common.Interfaces.Entities.Users.DTOs;
using FluentValidation;

namespace Application.Common.Validations.UserValidations;

public class RegisterUserValidator : AbstractValidator<CreateUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(user => user.FullName)
            .NotEmpty()
            .WithMessage("Campo de e-mail é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");
        
        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage("Campo de e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Endereço de e-mail inválido.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");
        
        RuleFor(user => user.PhoneNumber)
            .NotEmpty()
            .WithMessage("Campo de telefone é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");
        
        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage("Campo de senha é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");

        RuleFor(user => user.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Campo de confirmar senha é obrigatório.")
            .Equal(user => user.Password)
            .WithMessage("Campo de senha e confirmar senha devem ser iguais.");

    }
}