using CleanArchitectureApi.Application.DTOs.Posts;
using CleanArchitectureApi.Application.Features.Posts.Commands.CreatePost;
using CleanArchitectureApi.Application.Features.Posts.Queries.GetAllPosts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArchitectureApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var result = await _mediator.Send(new GetAllPostsQuery());
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        
        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        var result = await _mediator.Send(new CreatePostCommand(request.Title, request.UserId, request.TagNames));
        
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        
        return CreatedAtAction(nameof(GetAllPosts), result.Data);
    }
}

public record CreatePostRequest(string Title, Guid UserId, List<string> TagNames);
