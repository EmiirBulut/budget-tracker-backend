using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Auth.DTOs;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        var storedToken = await _userRepository.GetActiveRefreshTokenByHashAsync(tokenHash, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(storedToken.UserId, cancellationToken);
        if (user is null)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        // Rotate: revoke the used token and issue a fresh pair.
        storedToken.Revoke();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var rawRefreshToken = _tokenService.GenerateRefreshToken();
        var newTokenHash = _tokenService.HashRefreshToken(rawRefreshToken);

        var newRefreshToken = RefreshToken.Create(
            userId: user.Id,
            tokenHash: newTokenHash,
            expiresAt: DateTime.UtcNow.AddDays(7));

        await _userRepository.AddRefreshTokenAsync(newRefreshToken, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token rotated for user: {UserId}", user.Id);

        return new AuthResponseDto(accessToken, rawRefreshToken, ExpiresIn: 15 * 60);
    }
}
