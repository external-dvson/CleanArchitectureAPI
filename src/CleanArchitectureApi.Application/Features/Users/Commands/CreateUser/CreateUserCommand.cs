using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Users;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(string Username) : IRequest<Result<UserDto>>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if username already exists
        if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
        {
            return Result<UserDto>.Failure($"Username '{request.Username}' already exists.");
        }

        var user = new User
        {
            Username = request.Username
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            CreatedAt = user.CreatedAt
        };

        return Result<UserDto>.Success(userDto);
    }
}
