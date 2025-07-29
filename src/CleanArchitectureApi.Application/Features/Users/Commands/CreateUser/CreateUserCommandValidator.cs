using CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;
using FluentValidation;

namespace CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters.")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores.");
    }
}
