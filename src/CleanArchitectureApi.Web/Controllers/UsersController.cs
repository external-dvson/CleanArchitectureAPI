using CleanArchitectureApi.Application.Features.Users.Commands.CreateUser;
using CleanArchitectureApi.Application.Features.Users.Commands.UpdateUser;
using CleanArchitectureApi.Application.Features.Users.Queries.GetAllUsers;
using CleanArchitectureApi.Application.Features.Users.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureApi.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery(id));

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var result = await _mediator.Send(new CreateUserCommand(request.Username, request.Bio));

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetUserById), new { id = result.Data!.Id }, result.Data);
    }
    
    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateUserProfile(Guid id, [FromBody] UpdateUserProfileRequest request)
    {
        var result = await _mediator.Send(new UpdateUserProfileCommand(id, request.Bio));

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Data);
    }
}

public record CreateUserRequest(string Username, string? Bio = null);
public record UpdateUserProfileRequest(string Bio);