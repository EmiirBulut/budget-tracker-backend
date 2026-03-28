using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Accounts.Commands;

public class ArchiveAccountCommandHandler : IRequestHandler<ArchiveAccountCommand>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<ArchiveAccountCommandHandler> _logger;

    public ArchiveAccountCommandHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        ILogger<ArchiveAccountCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(ArchiveAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var account = await _accountRepository.GetByIdAsync(request.AccountId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountId);

        account.Archive();
        await _accountRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Account archived: {AccountId} for user {UserId}", account.Id, userId);
    }
}
