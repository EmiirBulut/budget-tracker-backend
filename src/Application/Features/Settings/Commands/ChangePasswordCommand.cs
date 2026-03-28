using MediatR;

namespace BudgetTracker.Application.Features.Settings.Commands;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;
