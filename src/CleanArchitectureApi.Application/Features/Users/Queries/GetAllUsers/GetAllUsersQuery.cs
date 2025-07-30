using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Users;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<Result<IEnumerable<UserDto>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetUsersWithProfileAsync(cancellationToken);

        var userDtos = users.Select(user => new UserDto
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
        });

        return Result<IEnumerable<UserDto>>.Success(userDtos);
    }
}
