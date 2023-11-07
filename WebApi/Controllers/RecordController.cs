﻿using Application.Features.Records.Create;
using Application.Features.Records.Delete;
using Application.Features.Records.Get;
using Application.Features.Records.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class RecordController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RecordController> _logger;

    public RecordController(IMediator mediator, ILogger<RecordController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Создает новую запись.")]
    [SwaggerResponse(StatusCodes.Status201Created, "Record Created Successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Request", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreateRecord(string? title, string text, DateTime deadline, bool isPrivate)
    {
        var currentUser = HttpContext.User;

        var command = new CreateRecordCommand
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
            _logger.LogError("Failed to create record:{Reasons}", response.Reasons);
            return BadRequest(response.Reasons);
        }

        _logger.LogInformation("Created Record: {Value}", response.Value);
        return Created($"/api/Record/{response.Value}", response.Value);
    }

    [HttpGet("{encodedGuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Получает запись по кодированному GUID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Record Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetRecord(string encodedGuid)
    {
        var currentUser = HttpContext.User;

        var request = new GetOneRecordByUrlRequest
        {
            EncodedGuid = encodedGuid,
            UserId = UserServices.GetCurrentUserId(currentUser)
        };

        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Retrieved record: {Id}", response.Value.Id);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve record: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Удаляет запись по ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Record Deleted Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteRecord(Guid id)
    {
        var user = HttpContext.User;

        var command = new DeleteRecordByIdCommand
        {
            RecordId = id,
            UserId = UserServices.GetCurrentUserId(user)
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Deleted record: {Id}", command.RecordId);
            return Ok(result.Value);
        }
        _logger.LogError("Failed to delete record: {Reasons}", result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet("allForAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает все записи.")]
    [SwaggerResponse(StatusCodes.Status200OK, "All Records Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Records Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllRecords()
    {
        var request = new GetAllRecordsRequest();
        var result = await _mediator.Send(request);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Retrieved all records");
            return Ok(result.Value);
        }
        _logger.LogError("Failed to retrieve all records: {Reasons}", result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet("{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Получает запись по ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Record Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetRecordById(Guid id)
    {
        var currentUser = HttpContext.User;

        var request = new GetRecordByIdRequest
        {
            RecordId = id,
            UserId = UserServices.GetCurrentUserId(currentUser)
        };

        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Retrieved record: {Id}", response.Value.Id);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve record: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получение своих записей.")]
    [SwaggerResponse(StatusCodes.Status200OK, "All Records Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllForUser()
    {
        var user = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(user);
        var request = new GetAllRecordsForUserRequest() { UserId = userId };
        var result = await _mediator.Send(request);
        if (result.IsSuccess) return Ok(result.Value);
        return BadRequest(result.Reasons);
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Изменение записи по ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful Record Change")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Update(Guid recordId, string? title, DateTime deadline, string? text, bool isPrivate)
    {
        var user = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(user);
        var request = new UpdateRecordByIdCommand
        {
            UserId = userId,
            RecordId = recordId,
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