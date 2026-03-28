using MediatR;

namespace BudgetTracker.Application.Features.Auth.Commands;

public record RevokeTokenCommand(string RefreshToken) : IRequest;
