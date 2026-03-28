namespace BudgetTracker.Application.Features.Auth.DTOs;

public record AuthResponseDto(string AccessToken, string RefreshToken, int ExpiresIn);
