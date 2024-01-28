#region usings
using Application.Contracts;
using Application.Features.Records.Create;
using Application.Features.Records.Delete;
using Application.Features.Records.Get;
using Application.Features.Records.Update;
using Domain.IServices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApi.Services;
#endregion 

namespace WebApi.Controllers;

[Route("api/records")]
[ApiController]
[Authorize(Roles = "User, Admin")]
public class RecordController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IQRCodeGeneratorService _QRCodeGeneratorService;
    private readonly ILogger<RecordController> _logger;

    public RecordController(IMediator mediator, ILogger<RecordController> logger, IQRCodeGeneratorService qRCodeGeneratorService)
    {
        _mediator = mediator;
        _logger = logger;
        _QRCodeGeneratorService = qRCodeGeneratorService;
    }

    // Endpoint to create a new record
    [HttpPost]
    [SwaggerOperation(Summary = "Creates a new record")]
    [SwaggerResponse(StatusCodes.Status201Created, "Record Created Successfully", typeof(CreatedResult))] // ToDo: use a more appropriate return type
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid Request", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid Request", typeof(ProblemDetails))]
    public async Task<IActionResult> CreateRecord(CreateRecordDto record)
    {
        var currentUser = HttpContext.User;

        var command = new CreateRecordCommand
        {
            UserId = UserServices.GetCurrentUserId(currentUser),
            Data = record,
        };

        var response = await _mediator.Send(command);
        if (response.IsFailed)
        {
            _logger.LogError("Failed to create record:{Reasons}", response.Reasons);
            return BadRequest(response.Reasons);
        }

        _logger.LogInformation("Created Record: {Value}", response.Value);

        // ToDo: use configuration or another approach (not hard code)
        var recordUrl = $"https://localhost:7056/api/records/encoded/{response.Value}";

        var qrCode = _QRCodeGeneratorService.GenerateQRCodeFromText(recordUrl);

        return Created(recordUrl, new { Url = recordUrl, QRCode = qrCode });
    }

    // Endpoint to get a record by encoded GUID
    [HttpGet("encoded/{encodedGuid}")]
    [SwaggerOperation(Summary = "Retrieves the entry by encoded GUID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Record Retrieved Successfully", typeof(RecordDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid Request", typeof(ProblemDetails))]
    public async Task<IActionResult> GetRecord(string encodedGuid)
    {
        var currentUser = HttpContext.User;

        var request = new GetRecordByUrlRequest
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

    // Endpoint to get all public records with pagination
    [HttpGet("public")]
    [SwaggerOperation(Summary = "Retrieves all public records")]
    [SwaggerResponse(StatusCodes.Status200OK, "All Records Retrieved Successfully", typeof(AllRecordsDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Records Not Found", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid Request", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllPublicRecords([FromQuery] GetAllPublicRecordsRequest request)
    {
        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Retrieved all public records");
            return Ok(response.Value);
        }
        _logger.LogError("Failed to retrieve all public records: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }


    // Endpoint to get all records for the current user
    [HttpGet("user")]
    [SwaggerOperation(Summary = "Receive your records")]
    [SwaggerResponse(StatusCodes.Status200OK, "All Records Retrieved Successfully", typeof(List<AllRecordsDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid Request", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAllUserRecords()
    {
        var user = HttpContext.User;
        var request = new GetAllUserRecordsRequest()
        {
            UserId = UserServices.GetCurrentUserId(user)
        };

        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("All Records Retrieved");
            return Ok(response.Value);
        }
        _logger.LogError("Failed to retrieve user records: {Reasons}", response.Reasons);
        return BadRequest(response.Reasons);
    }

    // Endpoint to get a record by ID
    [HttpGet("{recordId}")]
    [SwaggerOperation(Summary = "Retrieves a record by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Record Retrieved Successfully", typeof(RecordDto))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid Request", typeof(ProblemDetails))]
    public async Task<IActionResult> GetRecordById(Guid recordId)
    {
        var currentUser = HttpContext.User;

        var request = new GetRecordByIdRequest
        {
            RecordId = recordId,
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

    // Endpoint to delete a record
    [HttpDelete("{recordId}")]
    [SwaggerOperation(Summary = "Deletes an record by ID")]
    [SwaggerResponse(StatusCodes.Status200OK, "Record Deleted Successfully",typeof(Guid))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(List<FluentResults.IReason>))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Invalid Request", typeof(ProblemDetails))]
    public async Task<IActionResult> DeleteRecord(Guid recordId)
    {
        var user = HttpContext.User;

        var command = new DeleteRecordByIdCommand
        {
            RecordId = recordId,
            UserId = UserServices.GetCurrentUserId(user)
        };

        var response = await _mediator.Send(command);

        if (response.IsSuccess)
        {
            _logger.LogInformation("Deleted record: {Id}", command.RecordId);
            return Ok(response.Value);
        }
        _logger.LogError("Failed to delete record: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    // Endpoint to update a record
    [HttpPut("{recordId}")]
    [SwaggerOperation(Summary = "")]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful Record Change")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Record Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Update(UpdateRecordDto record)
    {
        var user = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(user);

        var request = new UpdateRecordByIdCommand
        {
            UserId = userId,
            Data = record
        };

        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Successful Record Change");
            return Ok(response.Value);
        }
        _logger.LogError("Failed to change record: {Reasons}", response.Reasons);
        return BadRequest(response.Reasons);
    }

    // Endpoint to get top 100 records
    [HttpGet("popularity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получение топ 100 записей.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Records Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Records Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> FetchTopRecordsByPopularity()
    {
        var request = new FetchByPopularityRequest();
        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Retrieved top 100 public records");
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve top 100 public records: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    // Endpoint to get records by title
    [HttpGet("title/{title}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получение записей по названию.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Records Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Records Not Found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> FetchRecordsByTitle(string title)
    {
        var request = new RecordByTitleRequest { RecordTitle = title };
        var response = await _mediator.Send(request);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Records Retrieved Successfully");
            return Ok(response.Value);
        }
        _logger.LogError("Failed to retriver records by title: {Reasons}", response.Reasons);
        return NotFound(response.Reasons);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получение всех публичных записей.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Records Retrieved Successfully")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Records Not Found", typeof(ValidationProblemDetails))]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllRecords()
    {
        var request = new GetAllRecordsRequest();
        var response = await _mediator.Send(request);
        return Ok(response.Value);
    }
}
