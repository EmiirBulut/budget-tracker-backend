using FluentValidation;

namespace BudgetTracker.Application.Features.Cards.Validators;

public class UpdateCardCommandValidator : AbstractValidator<Commands.UpdateCardCommand>
{
    public UpdateCardCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Last4Digits).NotEmpty().Length(4).Matches(@"^\d{4}$").WithMessage("Last 4 digits must be exactly 4 numeric characters.");
        RuleFor(x => x.ExpiryDate).NotEmpty().MaximumLength(7);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CreditLimit).GreaterThan(0).When(x => x.CreditLimit.HasValue);
    }
}
