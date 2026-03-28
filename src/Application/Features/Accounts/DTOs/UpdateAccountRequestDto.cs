using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Application.Features.Accounts.DTOs;

public record UpdateAccountRequestDto(
    string Name,
    AccountType Type,
    Currency Currency);
