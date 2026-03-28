using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Accounts.DTOs;

public record AccountDto(
    Guid Id,
    string Name,
    AccountType Type,
    Currency Currency,
    decimal Balance,
    bool IsArchived,
    DateTime CreatedAt);
