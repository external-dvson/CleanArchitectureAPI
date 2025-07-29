using CleanArchitectureApi.Domain.Common;
using System;
using System.Collections.Generic;

namespace CleanArchitectureApi.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string Username { get; set; } = string.Empty;
    
    // Navigation properties
    public UserProfile? Profile { get; set; }
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}
