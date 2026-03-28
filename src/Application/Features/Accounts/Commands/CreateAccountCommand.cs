using FluentValidation;
using MediatR;
using BudgetTracker.Application.Features.Accounts.DTOs;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Accounts.Commands;

public record CreateAccountCommand(
    string Name,
    AccountType Type,
    Currency Currency,
    decimal InitialBalance) : IRequest<AccountDto>;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Account name is required.")
            .MaximumLength(100).WithMessage("Account name must not exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Account type is invalid.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Currency is invalid.");

        RuleFor(x => x.InitialBalance)
            .GreaterThanOrEqualTo(0).WithMessage("Initial balance must not be negative.");
    }
}
