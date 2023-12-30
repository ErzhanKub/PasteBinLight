using Application.Contracts;
using Application.Features.Users.Create;
using Application.Features.Users.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // Endpoint to create a new user
    [AllowAnonymous]
    [HttpPost("users")]
    [SwaggerOperation(Summary = "Creates a new user")]
    [SwaggerResponse(StatusCodes.Status201Created, "User Created Successfully", typeof(UserDto))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Request", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error / Invalid data", typeof(ProblemDetails))]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Created user: {Id}", result.Value.Id);
            return Created($"/api/auth/{result.Value.Id}", result.Value);
        }
        _logger.LogError("Failed to create user: {Reasons}", result.Reasons);
        return BadRequest(result.Reasons);
    }

    // Endpoint to create a new session (login)
    [AllowAnonymous]
    [HttpPost("sessions")]
    [SwaggerOperation(Summary = "Sign in")]
    [SwaggerResponse(StatusCodes.Status200OK, "Login successful")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid Request", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error / Invalid data", typeof(ProblemDetails))]
    public async Task<IActionResult> CreateSession(UserLoginRequest request)
    {
        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Created session for user: {UserId}", response.Value.UserId);
            return Ok(response.Value.Token);
        }
        _logger.LogError("Failed to create session for user: {UserId}, {Reasons}", response.Value.UserId, response.Reasons);
        return Unauthorized(response.Reasons);
    }
}
