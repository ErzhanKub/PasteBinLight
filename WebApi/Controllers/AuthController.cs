using Application.Features.Users.Create;
using Application.Features.Users.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("users")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Создает нового пользователя.")]
    [SwaggerResponse(StatusCodes.Status201Created, "User Created Successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Request", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Created user: {Id}", result.Value.Id);
            return Created($"/api/Auth/{result.Value.Id}", result.Value);
        }
        _logger.LogError("Failed to create user: {Reasons}", result.Reasons);
        return BadRequest(result.Reasons);
    }

    [AllowAnonymous]
    [HttpPost("sessions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Login.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Login successful")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid Request", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreateSession(LoginRequest request)
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
