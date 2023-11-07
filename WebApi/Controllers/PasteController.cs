using Application.Features.Postes.Create;
using Application.Features.Postes.Delete;
using Application.Features.Postes.Get;
using Application.Features.Postes.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class PasteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PasteController> _logger;

    public PasteController(IMediator mediator, ILogger<PasteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Создает новую запись.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Paste Created Successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Request", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreatePoste(string? title, string text, DateTime deadline, bool isPrivate)
    {
        var currentUser = HttpContext.User;

        var command = new CreatePasteCommand
        {
            UserId = UserServices.GetCurrentUserId(currentUser),
            Title = title,
            Text = text,
            DeadLine = deadline,
            IsPrivate = isPrivate
        };

        var response = await _mediator.Send(command);
        if (response.IsFailed)
        {
            _logger.LogError("Failed to create paste:{Reasons}", response.Reasons);
            return BadRequest(response.Reasons);
        }

        _logger.LogInformation("Created paste: {Value}", response.Value);
        return Created($"/api/Poste/{response.Value}", response.Value);
    }

    [HttpGet("{encodedGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Получает запись по кодированному GUID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paste Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Paste Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetPoste(string encodedGuid)
    {
        var currentUser = HttpContext.User;

        var request = new GetOnePasteByUrlRequest
        {
            EncodedGuid = encodedGuid,
            UserId = UserServices.GetCurrentUserId(currentUser)
        };

        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Retrieved paste: {Id}", response.Value.Id);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve paste: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Удаляет запись по ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paste Deleted Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Paste Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeletePoste(Guid id)
    {
        var user = HttpContext.User;

        var command = new DeletePasteByIdsCommand
        {
            PosteId = id,
            UserId = UserServices.GetCurrentUserId(user)
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Deleted paste: {Id}", command.PosteId);
            return Ok(result.Value);
        }
        _logger.LogError("Failed to delete paste: {Reasons}", result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet("allForAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает все записи.")]
    [SwaggerResponse(StatusCodes.Status200OK, "All Pastes Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Pastes Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllPostes()
    {
        var request = new GetAllPasteRequest();
        var result = await _mediator.Send(request);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Retrieved all pastes");
            return Ok(result.Value);
        }
        _logger.LogError("Failed to retrieve all pastes: {Reasons}", result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet("{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Получает запись по ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paste Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Paste Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetPasteById(Guid id)
    {
        var currentUser = HttpContext.User;

        var request = new GetPasteByIdRequest
        {
            PasteId = id,
            UserId = UserServices.GetCurrentUserId(currentUser)
        };

        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Retrieved paste: {Id}", response.Value.Id);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve paste: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получение своих записей.")]
    [SwaggerResponse(StatusCodes.Status200OK, "All Pastes Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Paste Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllForUser()
    {
        var user = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(user);
        var request = new GetAllPastesForUserRequest() { UserId = userId };
        var result = await _mediator.Send(request);
        if (result.IsSuccess) return Ok(result.Value);
        return BadRequest(result.Reasons);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Изменение записи по ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful Paste Change")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Paste Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Update(Guid pasteId, string? title, DateTime deadline, string? text, bool isPrivate)
    {
        var user = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(user);
        var request = new UpdatePasteByIdCommand
        {
            UserId = userId,
            PosteId = pasteId,
            Title = title,
            DeadLine = deadline,
            IsPrivate = isPrivate,
            Text = text,
        };
        var result = await _mediator.Send(request);
        if (result.IsSuccess)
            return Ok(result.Value);
        return BadRequest(result.Reasons);
    }
}
