using System.IdentityModel.Tokens.Jwt;
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
        // In .NET 8+, JwtBearerHandler uses JsonWebTokenHandler with MapInboundClaims=false,
        // so the "sub" claim is NOT automatically mapped to ClaimTypes.NameIdentifier.
        // We read it directly by its JWT claim name.
        var sub = _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (sub is null || !Guid.TryParse(sub, out var userId))
            throw new UnauthorizedException("User identity could not be determined.");

        return userId;
    }
}
