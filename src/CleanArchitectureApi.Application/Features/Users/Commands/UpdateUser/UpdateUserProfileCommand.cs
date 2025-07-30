using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Users;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserProfileCommand(Guid UserId, string Bio) : IRequest<Result<UserDto>>; 

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetWithProfileForUpdateAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found.");
        }

        if (user.Profile == null)
        {
            user.UpdateProfile(request.Bio);
            await _unitOfWork.Users.UpdateUserWithNewProfileAsync(user, user.Profile!, cancellationToken);
        }
        else
        {
            // Use domain method for existing profile
            user.UpdateProfile(request.Bio);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            Profile = user.Profile != null ? new UserProfileDto
            {
                Id = user.Profile.Id,
                Bio = user.Profile.Bio,
                UserId = user.Profile.UserId,
            } : null
        };

        return Result<UserDto>.Success(userDto);
    }
}