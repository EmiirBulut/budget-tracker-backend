using FluentValidation;

namespace BudgetTracker.Application.Features.Installments.Validators;

public class CreateInstallmentPlanCommandValidator : AbstractValidator<Commands.CreateInstallmentPlanCommand>
{
    public CreateInstallmentPlanCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TotalAmount).GreaterThan(0);
        RuleFor(x => x.MonthlyPayment).GreaterThan(0);
        RuleFor(x => x.NumberOfMonths).InclusiveBetween(1, 360);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.CardId).NotEmpty();
    }
}
