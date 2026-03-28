using FluentValidation;
using MediatR;

namespace BudgetTracker.Application.Features.Accounts.Commands;

public record ArchiveAccountCommand(Guid AccountId) : IRequest;

public class ArchiveAccountCommandValidator : AbstractValidator<ArchiveAccountCommand>
{
    public ArchiveAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty().WithMessage("Account ID is required.");
    }
}
