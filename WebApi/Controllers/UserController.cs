using Application.Features.Users;
using Application.Features.Users.Delete;
using Application.Features.Users.Get;
using Application.Features.Users.Update;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Web;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Обновляет пользователя по Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully updated", typeof(User))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateUserById(Guid id, string? username, string? password, string? email, int userRole)
    {
        var request = new UpdateUserByIdCommand
        {
            Id = id,
            Username = username,
            Password = password,
            Email = email,
            UserRole = userRole,
        };

        _logger.LogInformation("Updating user by Id: {Id}", id);
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User with Id: {Id} successfully updated", id);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to update user with Id: {Id}. Reasons: {Reasons}", id, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpPut("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Обновляет текущего пользователя.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Current user successfully updated", typeof(User))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Current user not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateCurrentUser(string? username, string? password, string? email)
    {
        var currentUser = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(currentUser);
        var request = new UpdateUserByIdCommand
        {
            Id = userId,
            Username = username,
            Password = password,
            Email = email,
            UserRole = 1,
        };

        _logger.LogInformation("Updating current user Id: {Id}", userId);
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Current user Id: {Id} successfully updated", userId);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to update current user with ID: {Id}. Reasons: {Reasons}", userId, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает пользователя по Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully retrieved", typeof(User))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var request = new GetOneUserRequest { Id = id };

        _logger.LogInformation("Retrieving user by ID: {Id}", id);
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User with Id: {Id} successfully retrieved", id);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to retrieve user with Id: {Id}. Reasons: {Reasons}", id, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet("{username}.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает пользователя по username.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully retrieved", typeof(User))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var request = new GetUserByUsernameRequest { Username = username };

        _logger.LogInformation("Retrieving user by username: {username}", username);
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User with username: {username} successfully retrieved", username);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to retrieve user with username: {username}. Reasons: {Reasons}", username, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает всех пользователей.")]
    [SwaggerResponse(StatusCodes.Status200OK, "All users successfully retrieved", typeof(List<User>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Users not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllUsers()
    {
        var request = new GetAllRequest();

        _logger.LogInformation("Retrieving all users");
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            _logger.LogInformation("All users successfully retrieved");
            return Ok(result.Value);
        }

        _logger.LogError("Failed to retrieve all users. Reasons: {Reasons}", result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Удаляет пользователя по Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteById(DeleteUsersByIdsCommand command)
    {
        _logger.LogInformation("Deleting user by ID: {Id}", command.Id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User with ID: {Id} successfully deleted", command.Id);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to delete user with ID: {Id}. Reasons: {Reasons}", command.Id, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpDelete("{username}.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Удаляет пользователя по Username.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteByUsername(DeleteByUsernameCommand command)
    {
        _logger.LogInformation("Deleting user by Username: {Username}", command.Username);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User with Username: {Username} successfully deleted", command.Username);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to delete user with Username: {Username}. Reasons: {Reasons}", command.Username, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Удаляет текущего пользователя.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Current user successfully deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Current user not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        var currentUser = HttpContext.User;
        var currentUserId = UserServices.GetCurrentUserId(currentUser);

        var command = new DeleteUsersByIdsCommand()
        {
            Id = new Guid[] { currentUserId },
        };

        _logger.LogInformation("Deleting current user by ID: {Id}", currentUserId);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            _logger.LogInformation("Current user with ID: {Id} successfully deleted", currentUserId);
            return Ok(result.Value);
        }

        _logger.LogError("Failed to delete current user with ID: {Id}. Reasons: {Reasons}", currentUserId, result.Reasons);
        return NotFound(result.Reasons);
    }

    [HttpPatch("confirm/{confirmToken}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Подтверждает электронную почту.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Mail confirmed", typeof(User))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Mail not confirmed", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> ConfirmEmail(string confirmToken)
    {
        var currentUser = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(currentUser);

        string decodedToken = HttpUtility.UrlDecode(confirmToken);

        var command = new ConfirmEmailCommand
        {
            UserId = userId,
            ConfirmToken = decodedToken
        };

        _logger.LogInformation("Confirming email for user by ID: {Id}", userId);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            _logger.LogInformation("Email for user with ID: {Id} successfully confirmed", userId);
            return Ok("Mail confirmed");
        }

        _logger.LogError("Failed to confirm email for user with ID: {Id}. Reasons: {Reasons}", userId, result.Reasons);
        return NotFound(result.Reasons);
    }
}
