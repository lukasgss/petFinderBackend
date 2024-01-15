using Application.Common.Interfaces.Entities.Alerts.Comments.DTOs;
using FluentValidation;

namespace Application.Common.Validations.Alerts.AlertComments;

public class CreateAlertCommentValidator : AbstractValidator<CreateAlertCommentRequest>
{
	public CreateAlertCommentValidator()
	{
		RuleFor(alert => alert.Content)
			.NotEmpty()
			.WithMessage("Campo de conteúdo é obrigatório.")
			.MaximumLength(500)
			.WithMessage("Máximo de 500 caracteres permitidos.");

		RuleFor(alert => alert.AlertId)
			.NotEmpty()
			.WithMessage("Campo de id do alerta é obrigatório.");
	}
}