using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;

namespace BudgetTracker.Application.Features.Auth.Commands;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RevokeTokenCommandHandler> _logger;

    public RevokeTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<RevokeTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await _userRepository.GetActiveRefreshTokenByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || storedToken.IsRevoked)
            return; // Already revoked or never existed — no information leaked.

        storedToken.Revoke();
        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token revoked for user: {UserId}", storedToken.UserId);
    }
}
