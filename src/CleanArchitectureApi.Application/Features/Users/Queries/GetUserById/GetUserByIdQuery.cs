using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Users;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<Result<UserDto>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetWithProfileAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return Result<UserDto>.Failure("User not found.");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            Profile = user.Profile != null ? new UserProfileDto
            {
                Id = user.Profile.Id,
                Bio = user.Profile.Bio,
                UserId = user.Profile.UserId
            } : null
        };

        return Result<UserDto>.Success(userDto);
    }
}
