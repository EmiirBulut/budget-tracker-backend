using MediatR;

namespace BudgetTracker.Application.Features.Settings.Commands;

public record UpdateProfileCommand(string Email) : IRequest;
