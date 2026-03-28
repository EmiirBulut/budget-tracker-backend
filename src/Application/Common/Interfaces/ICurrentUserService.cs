namespace BudgetTracker.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid GetCurrentUserId();
}
