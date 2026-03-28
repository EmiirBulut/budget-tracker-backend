using FluentValidation;
using MediatR;
using BudgetTracker.Application.Features.Accounts.DTOs;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Accounts.Commands;

public record UpdateAccountCommand(
    Guid AccountId,
    string Name,
    AccountType Type,
    Currency Currency) : IRequest<AccountDto>;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Account name is required.")
            .MaximumLength(100).WithMessage("Account name must not exceed 100 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Account type is invalid.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Currency is invalid.");
    }
}
