using Application.Features.Postes.Create;
using Application.Features.Postes.Delete;
using Application.Features.Postes.Get;
using Application.Features.Postes.Update;
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

        [HttpPost]
        public async Task<IActionResult> CreatePoste(CreatePosteCommand command)
        {
            var currentUser = HttpContext.User;
            command.UserId = Helper.GetCurrentUserId(currentUser);

            var response = await _mediator.Send(command);
            if (response.IsFailed)
            {
                _logger.LogError("Failed to create poste:{Reasons}", response.Reasons);
                return BadRequest(response.Reasons);
            }

            _logger.LogInformation("Created poste: {Value}", response.Value);
            return Created($"/api/Poste/{response.Value}", response.Value);
        }

        [HttpGet("{encodedGuid}")]
        public async Task<IActionResult> GetPoste(string encodedGuid)
        {
            var currentUser = HttpContext.User;

            var request = new GetOnePosteByUrlRequest
            {
                EncodedGuid = encodedGuid,
                UserId = Helper.GetCurrentUserId(currentUser)
            };

            var response = await _mediator.Send(request);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Retrieved poste: {Id}", response.Value.Id);
                return Ok(response.Value);
            }

            _logger.LogError("Failed to retrieve poste: {Reasons}", response.Reasons);
            return NotFound(response.Reasons);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoste(Guid id)
        {
            var user = HttpContext.User;

            var command = new DeletePosteByIdsCommand
            {
                PosteId = id,
                UserId = Helper.GetCurrentUserId(user)
            };
        
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                _logger.LogInformation("Deleted poste: {Id}", command.PosteId);
                return Ok(result.Value);
            }
            _logger.LogError("Failed to delete poste: {Reasons}", result.Reasons);
            return NotFound(result.Reasons);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllPostes()
        {
            var request = new GetAllPosteRequest();
            var result = await _mediator.Send(request);
            if (result.IsSuccess)
            {
                _logger.LogInformation("Retrieved all postes");
                return Ok(result.Value);
            }
            _logger.LogError("Failed to retrieve all postes: {Reasons}", result.Reasons);
            return NotFound(result.Reasons);
        }
    }
}
