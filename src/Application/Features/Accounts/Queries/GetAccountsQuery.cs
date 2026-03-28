using MediatR;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Accounts.DTOs;

namespace BudgetTracker.Application.Features.Accounts.Queries;

public record GetAccountsQuery : IRequest<List<AccountDto>>;

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, List<AccountDto>>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAccountsQueryHandler(
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService)
    {
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        var accounts = await _accountRepository.GetAllByUserIdAsync(userId, cancellationToken);

        return accounts.Select(a => new AccountDto(
            a.Id,
            a.Name,
            a.Type,
            a.Currency,
            a.Balance,
            a.IsArchived,
            a.CreatedAt)).ToList();
    }
}
