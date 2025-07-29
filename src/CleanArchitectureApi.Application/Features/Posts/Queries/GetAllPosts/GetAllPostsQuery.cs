using CleanArchitectureApi.Application.Common.Models;
using CleanArchitectureApi.Application.DTOs.Posts;
using CleanArchitectureApi.Domain.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Application.Features.Posts.Queries.GetAllPosts;

public record GetAllPostsQuery : IRequest<Result<IEnumerable<PostDto>>>;

public class GetAllPostsQueryHandler : IRequestHandler<GetAllPostsQuery, Result<IEnumerable<PostDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPostsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<PostDto>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await _unitOfWork.Posts.GetWithUserAsync(cancellationToken);

        var postDtos = posts.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            UserId = post.UserId,
            Username = post.User?.Username ?? string.Empty,
            CreatedAt = post.CreatedAt,
            Tags = post.PostTags.Select(pt => new TagDto
            {
                Id = pt.Tag.Id,
                Name = pt.Tag.Name
            }).ToList()
        });

        return Result<IEnumerable<PostDto>>.Success(postDtos);
    }
}
