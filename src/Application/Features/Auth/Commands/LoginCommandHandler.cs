using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Application.Features.Auth.DTOs;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Use the same error message for both "not found" and "wrong password"
        // to avoid leaking which emails are registered.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var (accessToken, refreshToken) = await IssueTokensAsync(user, cancellationToken);

        _logger.LogInformation("User logged in: {UserId}", user.Id);

        return new AuthResponseDto(accessToken, refreshToken, ExpiresIn: 15 * 60);
    }

    private async Task<(string AccessToken, string RawRefreshToken)> IssueTokensAsync(
        User user, CancellationToken cancellationToken)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var rawRefreshToken = _tokenService.GenerateRefreshToken();
        var tokenHash = _tokenService.HashRefreshToken(rawRefreshToken);

        var refreshToken = RefreshToken.Create(
            userId: user.Id,
            tokenHash: tokenHash,
            expiresAt: DateTime.UtcNow.AddDays(7));

        await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return (accessToken, rawRefreshToken);
    }
}
