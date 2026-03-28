using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Accounts.DTOs;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Accounts.Queries;

public record GetAccountByIdQuery(Guid AccountId) : IRequest<AccountDto>;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAccountByIdQueryHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<AccountDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var account = await _accountRepository.GetByIdAsync(request.AccountId, userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountId);

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
