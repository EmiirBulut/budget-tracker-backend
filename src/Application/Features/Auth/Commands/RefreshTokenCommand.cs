using MediatR;
using BudgetTracker.Application.Features.Auth.DTOs;

namespace BudgetTracker.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
