using CleanArchitectureApi.Application.Common.Interfaces;
using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Posts;
using CleanArchitectureApi.Domain.Entities;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Application.Features.Posts.Commands.CreatePost;

public record CreatePostCommand(string Title, Guid UserId, List<string> TagNames) : ITransactionalCommand<Result<PostDto>>;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Result<PostDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePostCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PostDto>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        // Check if user exists
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<PostDto>.Failure("User not found.");
        }

        var post = new Post
        {
            Title = request.Title,
            UserId = request.UserId
        };

        // Handle tags and create PostTag relationships
        foreach (var tagName in request.TagNames)
        {
            var tag = await _unitOfWork.Tags.GetByNameAsync(tagName, cancellationToken);
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                await _unitOfWork.Tags.AddAsync(tag, cancellationToken);
                // Save to get Tag ID
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            // Add PostTag to Post's navigation property
            post.PostTags.Add(new PostTag
            {
                Post = post,
                Tag = tag,
                PostId = post.Id,    // Will be set by EF
                TagId = tag.Id       // Already available
            });
        }

        await _unitOfWork.Posts.AddAsync(post, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var postDto = new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            UserId = post.UserId,
            Username = user.Username,
            CreatedAt = post.CreatedAt,
            Tags = request.TagNames.Select(name => new TagDto { Name = name }).ToList()
        };

        return Result<PostDto>.Success(postDto);
    }
}
