using FluentValidation;

namespace BudgetTracker.Application.Features.Transactions.Validators;

public class CreateTransactionCommandValidator : AbstractValidator<Commands.CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Date).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.AccountId.HasValue || x.CardId.HasValue)
            .WithMessage("Either an account or a card must be linked to a transaction.");
    }
}
