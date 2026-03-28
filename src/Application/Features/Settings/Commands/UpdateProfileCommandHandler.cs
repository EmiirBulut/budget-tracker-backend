using MediatR;
using Microsoft.Extensions.Logging;
using BudgetTracker.Application.Common.Interfaces;
using BudgetTracker.Domain.Exceptions;

namespace BudgetTracker.Application.Features.Settings.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetCurrentUserId();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.User), userId);

        // Check new email is not taken by another user.
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null && existing.Id != userId)
            throw new ConflictException("This email is already in use.");

        user.UpdateEmail(request.Email);
        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile updated for user: {UserId}", userId);
    }
}
