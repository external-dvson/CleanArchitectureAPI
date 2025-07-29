using CleanArchitectureApi.Domain.Common;
using System.Collections.Generic;

namespace CleanArchitectureApi.Domain.Entities;

public class Tag : BaseEntity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
