using FluentValidation;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Cards.Validators;

public class CreateCardCommandValidator : AbstractValidator<Commands.CreateCardCommand>
{
    public CreateCardCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Last4Digits).NotEmpty().Length(4).Matches(@"^\d{4}$").WithMessage("Last 4 digits must be exactly 4 numeric characters.");
        RuleFor(x => x.ExpiryDate).NotEmpty().MaximumLength(7);
        RuleFor(x => x.Color).NotEmpty().MaximumLength(20);

        RuleFor(x => x.CreditLimit)
            .GreaterThan(0).When(x => x.CreditLimit.HasValue)
            .NotNull().When(x => x.CardCategory == CardCategory.Credit)
            .WithMessage("Credit limit is required for credit cards.");

        RuleFor(x => x.LinkedAccountId)
            .NotNull().When(x => x.CardCategory == CardCategory.Debit)
            .WithMessage("A linked account is required for debit cards.");
    }
}
