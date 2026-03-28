using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Accounts.DTOs;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Accounts.Commands;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateAccountCommandHandler> _logger;

    public UpdateAccountCommandHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateAccountCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<AccountDto> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var account = await _accountRepository.GetByIdAsync(request.AccountId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountId);

        account.Update(request.Name, request.Type, request.Currency);
        await _accountRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account updated: {AccountId} for user {UserId}", account.Id, userId);

        return new AccountDto(
            account.Id,
            account.Name,
            account.Type,
            account.Currency,
            account.Balance,
            account.IsArchived,
            account.CreatedAt);
    }
}
