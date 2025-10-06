using CleanArchitectureApi.Application.Common.Interfaces;
using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Users;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;

namespace CleanArchitectureApi.Application.Features.Users.Commands.BulkReplaceUsers;

public record BulkReplaceUsersCommand(List<BulkUserData> Users) : ITransactionalCommand<Result<List<UserDto>>>;

public record BulkUserData(string Username, string? Bio = null);

public class BulkReplaceUsersCommandHandler : IRequestHandler<BulkReplaceUsersCommand, Result<List<UserDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public BulkReplaceUsersCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<UserDto>>> Handle(BulkReplaceUsersCommand request, CancellationToken cancellationToken)
    {
        // Validate input
        if (!request.Users.Any())
        {
            return Result<List<UserDto>>.Failure("No users provided for bulk replace operation.");
        }

        // Check for duplicate usernames in the request
        var duplicateUsernames = request.Users
            .GroupBy(u => u.Username)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateUsernames.Any())
        {
            return Result<List<UserDto>>.Failure($"Duplicate usernames found: {string.Join(", ", duplicateUsernames)}");
        }

        // Method 1: Using ExecuteSqlRawAsync for TRUNCATE (faster)
        await _unitOfWork.Users.ExecuteSqlRawAsync("TRUNCATE TABLE Users", cancellationToken);
        
        // Alternative Method 2: Using DeleteAllAsync (safer for foreign key constraints)
        // await _unitOfWork.Users.DeleteAllAsync(cancellationToken);
        // await _unitOfWork.SaveChangesAsync(cancellationToken); // Need to save after delete

        // Prepare entities for bulk insert
        var usersToInsert = request.Users.Select(userData =>
        {
            var user = new User { Username = userData.Username };
            
            if (!string.IsNullOrEmpty(userData.Bio))
            {
                user.CreateProfile(userData.Bio);
            }
            
            return user;
        }).ToList();

        // Bulk insert using AddRangeAsync
        await _unitOfWork.Users.AddRangeAsync(usersToInsert, cancellationToken);
        
        // Save all changes (this will be committed by TransactionBehavior)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Prepare response DTOs
        var userDtos = usersToInsert.Select(user => new UserDto
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
        }).ToList();

        return Result<List<UserDto>>.Success(userDtos);
    }
}