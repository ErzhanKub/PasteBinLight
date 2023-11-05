using Application.Contracts;
using Application.Features.Users.Create;
using Application.Features.Users.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
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
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Created user: {Id}", result.Value.Id);
                return CreatedAtAction(nameof(UserDto), result.Value);
            }
            _logger.LogError("Failed to create user: {Reasons}", result.Reasons);
            return BadRequest(result.Reasons);
        }

        [AllowAnonymous]
        [HttpPost("sessions")]
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
}
