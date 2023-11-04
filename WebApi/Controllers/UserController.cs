using Application.Features.Users;
using Application.Features.Users.Delete;
using Application.Features.Users.Get;
using Application.Features.Users.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(DeleteUsersByIdsCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Reasons);
        }


        [HttpDelete("deleteCurrentUser")]
        public async Task<IActionResult> DeleteCurrentUser()
        {
            var currentUser = HttpContext.User;
            var currentUserId = Helper.GetCurrentUserId(currentUser);

            var command = new DeleteUsersByIdsCommand()
            {
                Id = new Guid[] { currentUserId },
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Reasons);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var request = new GetOneUserRequest { Id = id };

            var result = await _mediator.Send(request);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Reasons);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            var request = new GetAllRequest();

            var result = await _mediator.Send(request);

            return Ok(result);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateById(Guid id, string? username, string? password, string? email, int userRole)
        {
            var request = new UpdateUserByIdCommand
            {
                Id = id,
                Username = username,
                Password = password,
                Email = email,
                UserRole = userRole,
            };

            var result = await _mediator.Send(request);

            if (result.IsSuccess)
                return Ok(result.Value);
            return BadRequest(result.Reasons);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCurrentUser(string? username, string? password, string? email)
        {
            var currentUser = HttpContext.User;
            var userId = Helper.GetCurrentUserId(currentUser);
            var request = new UpdateUserByIdCommand
            {
                Id = userId,
                Username = username,
                Password = password,
                Email = email,
                UserRole = 1,
            };

            var result = await _mediator.Send(request);

            if (result.IsSuccess)
                return Ok(result.Value);

            return BadRequest(result.Reasons);
        }

        [HttpPatch("{confirmToken}")]
        public async Task<IActionResult> ConfirmEmail(string confirmToken)
        {
            var currentUser = HttpContext.User;
            var userId = Helper.GetCurrentUserId(currentUser);

            var command = new ConfirmEmailCommand
            {
                UserId = userId,
                ConfirmToken = confirmToken
            };

            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok("Mail confirmed");
            return BadRequest(result.Reasons);
        }
    }
}

