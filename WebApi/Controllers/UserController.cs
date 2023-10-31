using Application.Features.Users.Delete;
using Application.Features.Users.Get;
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
                Id = new Guid[] {currentUserId},
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
    }
}
