using FluentValidation;

namespace BudgetTracker.Application.Features.Settings.Validators;

public class UpdateProfileCommandValidator : AbstractValidator<Commands.UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
    }
}

public class ChangePasswordCommandValidator : AbstractValidator<Commands.ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
