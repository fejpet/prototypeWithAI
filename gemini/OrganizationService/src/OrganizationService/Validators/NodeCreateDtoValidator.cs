using FluentValidation;
using OrganizationService.DTOs;

namespace OrganizationService.Validators;

public class NodeCreateDtoValidator : AbstractValidator<NodeCreateDto>
{
    public NodeCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("A név megadása kötelező.")
            .MaximumLength(100).WithMessage("A név maximum 100 karakter lehet.");

        RuleFor(x => x.ParentId)
            .NotEmpty().WithMessage("A szülő azonosító megadása kötelező.");
    }
}
