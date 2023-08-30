using Application.Common.Interfaces.Entities.Pets.DTOs;
using FluentValidation;

namespace Application.Common.Validations.PetValidations;

public class EditPetValidator : AbstractValidator<EditPetRequest>
{
    public EditPetValidator()
    {
        RuleFor(pet => pet.Id)
            .NotEmpty()
            .WithMessage("Campo de id é obrigatório.");
        
        RuleFor(pet => pet.Name)
            .NotEmpty()
            .WithMessage("Campo de nome é obrigatório.")
            .MaximumLength(255)
            .WithMessage("Máximo de 255 caracteres permitidos.");

        RuleFor(pet => pet.Observations)
            .MaximumLength(500)
            .WithMessage("Máximo de 500 caracteres permitidos.");

        RuleFor(pet => pet.BreedId)
            .NotNull()
            .WithMessage("Campo de raça é obrigatório.");

        RuleFor(pet => pet.SpeciesId)
            .NotNull()
            .WithMessage("Campo de espécie é obrigatório.");

        RuleFor(pet => pet.ColorIds)
            .NotEmpty()
            .WithMessage("Campo de cores é obrigatório.");
    }
}