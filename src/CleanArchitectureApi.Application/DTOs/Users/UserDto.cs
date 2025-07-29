using System;

namespace CleanArchitectureApi.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserProfileDto? Profile { get; set; }
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Bio { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
