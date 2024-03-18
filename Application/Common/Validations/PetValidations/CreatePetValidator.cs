using Application.Common.Interfaces.Entities.Pets.DTOs;
using Domain.Enums;
using FluentValidation;

namespace Application.Common.Validations.PetValidations;

public class CreatePetValidator : AbstractValidator<CreatePetRequest>
{
	public CreatePetValidator()
	{
		RuleFor(pet => pet.Name)
			.NotEmpty()
			.WithMessage("Campo de nome é obrigatório.")
			.MaximumLength(255)
			.WithMessage("Máximo de 255 caracteres permitidos.");

		RuleFor(pet => pet.Observations)
			.MaximumLength(500)
			.WithMessage("Máximo de 500 caracteres permitidos.");

		RuleFor(pet => pet.Gender)
			.NotNull()
			.WithMessage("Campo de gênero é obrigatório.")
			.Must(gender => Enum.IsDefined(typeof(Gender), gender))
			.WithMessage("Valor inválido como gênero.");

		RuleFor(pet => pet.Size)
			.NotNull()
			.WithMessage("Campo de porte é obrigatório.")
			.Must(size => Enum.IsDefined(typeof(Size), size))
			.WithMessage("Valor inválido como porte.");

		RuleFor(pet => pet.Age)
			.NotNull()
			.WithMessage("Campo de idade é obrigatório.")
			.Must(age => Enum.IsDefined(typeof(Age), age))
			.WithMessage("Valor inválido como idade.");

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