using CleanArchitectureApi.Domain.Common;
using System;
using System.Collections.Generic;

namespace CleanArchitectureApi.Domain.Entities;

public class Post : BaseEntity, IAggregateRoot
{
    public string Title { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
