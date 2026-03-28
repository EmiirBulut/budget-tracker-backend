using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Accounts.DTOs;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Application.Features.Accounts.Commands;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateAccountCommandHandler> _logger;

    public CreateAccountCommandHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        ILogger<CreateAccountCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var account = Account.Create(
            userId,
            request.Name,
            request.Type,
            request.Currency,
            request.InitialBalance);

        await _accountRepository.AddAsync(account, cancellationToken);
        await _accountRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account created: {AccountId} for user {UserId}", account.Id, userId);

        return ToDto(account);
    }

    private static AccountDto ToDto(Account a) =>
        new(a.Id, a.Name, a.Type, a.Currency, a.Balance, a.IsArchived, a.CreatedAt);
}
