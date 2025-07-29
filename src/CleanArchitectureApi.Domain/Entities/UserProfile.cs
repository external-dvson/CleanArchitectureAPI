using CleanArchitectureApi.Domain.Common;
using System;

namespace CleanArchitectureApi.Domain.Entities;

public class UserProfile : BaseEntity
{
    public string Bio { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
