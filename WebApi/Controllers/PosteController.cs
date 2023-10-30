using Application.Features.Postes.Create;
using Application.Features.Postes.Get.GetOne;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PosteController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PosteController> _logger;

        public PosteController(IMediator mediator, ILogger<PosteController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreatePosteCommand command)
        {
            var currentUser = HttpContext.User;
            command.UserId = Helper.GetCurrentUserId(currentUser);

            var result = await _mediator.Send(command);
            if (result.IsFailed)
                return BadRequest(result.Reasons);
            return Created($"/api/Poste/{result.Value}", result.Value);
        }

        [HttpGet("{url}")]
        public async Task<IActionResult> GetByUrl(Uri url)
        {
            var request = new GetOnePosteByUrlRequest {Url = url };
            var response = await _mediator.Send(request);
            if (response.IsSuccess)
                return Ok(response.Value);
            return BadRequest(response.Reasons);
        }

        private bool AccessCheck(Guid id)
        {
            var currentUser = HttpContext.User;
            var userData = Helper.GetUserDetails(currentUser);
            var isAdmin = userData.Item2 == "Admin";

            if (!isAdmin && userData.Item1 != id.ToString())
            {
                return false;
            }

            return true;
        }
    }
}
