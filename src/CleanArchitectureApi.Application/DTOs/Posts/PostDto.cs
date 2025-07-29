using System;
using System.Collections.Generic;

namespace CleanArchitectureApi.Application.DTOs.Posts;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<TagDto> Tags { get; set; } = new();
}

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
