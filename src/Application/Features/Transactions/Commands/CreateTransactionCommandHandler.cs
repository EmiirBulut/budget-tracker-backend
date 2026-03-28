using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Transactions.DTOs;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Transactions.Commands;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<CreateTransactionCommandHandler> _logger;

    public CreateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ICurrentUserService currentUserService,
        ILogger<CreateTransactionCommandHandler> logger)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TransactionDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();

        // Update account balance when an account is linked.
        if (request.AccountId.HasValue)
        {
            var account = await _accountRepository.GetByIdAsync(request.AccountId.Value, userId, cancellationToken)
                ?? throw new NotFoundException(nameof(Account), request.AccountId.Value);

            if (request.Type == TransactionType.Income)
                account.UpdateBalance(request.Amount);
            else
                account.UpdateBalance(-request.Amount);

            await _accountRepository.SaveChangesAsync(cancellationToken);
        }

        var transaction = Transaction.Create(
            userId, request.AccountId, request.CardId,
            request.Type, request.Amount,
            request.Category, request.Description,
            request.Date);

        await _transactionRepository.AddAsync(transaction, cancellationToken);
        await _transactionRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Transaction created: {TransactionId} for user: {UserId}", transaction.Id, userId);

        return new TransactionDto(
            transaction.Id, transaction.AccountId, transaction.CardId,
            transaction.Type.ToString(), transaction.Amount,
            transaction.Category, transaction.Description,
            transaction.Date, transaction.InstallmentPlanId, transaction.CreatedAt);
    }
}
