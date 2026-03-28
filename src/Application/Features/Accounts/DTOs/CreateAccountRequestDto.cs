using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Accounts.DTOs;

public record CreateAccountRequestDto(
    string Name,
    AccountType Type,
    Currency Currency,
    decimal InitialBalance);
