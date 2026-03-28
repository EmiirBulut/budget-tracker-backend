using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetCurrentUserId()
    {
        var sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (sub is null || !Guid.TryParse(sub, out var userId))
            throw new UnauthorizedException("User identity could not be determined.");

        return userId;
    }
}
