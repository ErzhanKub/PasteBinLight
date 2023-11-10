﻿using Application.Features.Users;
using Application.Features.Users.Delete;
using Application.Features.Users.Get;
using Application.Features.Users.Update;
using Domain.Entities;
using Domain.IServices;
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
    private readonly ITelegramService _telegramService;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger, ITelegramService telegramService)
    {
        _mediator = mediator;
        _logger = logger;
        _telegramService = telegramService;
    }

    [HttpPut("users/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Обновляет пользователя по Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully updated")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateUserById(Guid id, string? username, string? password, string? email, int userRole)
    {
        var request = new UpdateUserByIdCommand
        {
            UserId = id,
            NewUsername = username,
            NewPassword = password,
            NewEmail = email,
            NewUserRole = userRole,
        };

        _logger.LogInformation("Updating user by Id: {Id}", id);
        var response = await _mediator.Send(request);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User with Id: {Id} successfully updated", id);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to update user with Id: {Id}. Reasons: {Reasons}", id, response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpPut("users/me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Обновляет текущего пользователя.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Current user successfully updated")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Current user not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateCurrentUser(string? username, string? password, string? email)
    {
        var currentUser = HttpContext.User;
        var userId = UserServices.GetCurrentUserId(currentUser);
        var request = new UpdateUserByIdCommand
        {
            UserId = userId,
            NewUsername = username,
            NewPassword = password,
            NewEmail = email,
            NewUserRole = 1,
        };

        _logger.LogInformation("Updating current user Id: {Id}", userId);
        var response = await _mediator.Send(request);

        if (response.IsSuccess)
        {
            _logger.LogInformation("Current user Id: {Id} successfully updated", userId);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to update current user with ID: {Id}. Reasons: {Reasons}", userId, response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpGet("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает пользователя по Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var request = new FetchUserByIdRequest
        {
            UserId = userId,
        };

        _logger.LogInformation("Retrieving user by ID: {Id}", request.UserId);
        var response = await _mediator.Send(request);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User with Id: {Id} successfully retrieved", response.Value.Id);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve user with Id: {Id}. Reasons: {Reasons}", response.Value.Id, response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpGet("users/username/{username}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает пользователя по username.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var request = new FetchUserByUsernameRequest { TargetUsername = username };

        _logger.LogInformation("Retrieving user by username: {username}", request.TargetUsername);
        var response = await _mediator.Send(request);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User with username: {username} successfully retrieved", response.Value.Username);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to retrieve user with username: {username}. Reasons: {Reasons}", response.Value.Username, response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpGet("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Получает всех пользователей.")]
    [SwaggerResponse(StatusCodes.Status200OK, "All users successfully retrieved")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Users not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetAllUsers([FromQuery]GetAllUsersRequest request)
    {
        _logger.LogInformation("Retrieving all users");
        var reponse = await _mediator.Send(request);

        if (reponse.IsSuccess)
        {
            _logger.LogInformation("All users successfully retrieved");
            return Ok(reponse.Value);
        }

        _logger.LogError("Failed to retrieve all users. Reasons: {Reasons}", reponse.Reasons);
        return NotFound(reponse.Reasons);
    }

    [HttpDelete("users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Удаляет пользователя по Id.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteById(DeleteUsersByIdsCommand command)
    {
        _logger.LogInformation("Deleting user by ID: {Id}", command.UserIds);
        var response = await _mediator.Send(command);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User with ID: {Id} successfully deleted", command.UserIds);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to delete user with ID: {Id}. Reasons: {Reasons}", command.UserIds, response.Reasons);
        return NotFound(response.Reasons);
    }

    [HttpDelete("users/me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Удаляет пользователя по Username.")]
    [SwaggerResponse(StatusCodes.Status200OK, "User successfully deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found", typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteByUsername(string username)
    {
        var command = new DeleteByUsernameCommand { TargetUsername = username };

        _logger.LogInformation("Deleting user by Username: {Username}", command.TargetUsername);
        var response = await _mediator.Send(command);

        if (response.IsSuccess)
        {
            _logger.LogInformation("User with Username: {Username} successfully deleted", command.TargetUsername);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to delete user with Username: {Username}. Reasons: {Reasons}", command.TargetUsername, response.Reasons);
        return NotFound(response.Reasons);
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
            UserIds = new Guid[] { currentUserId },
        };

        _logger.LogInformation("Deleting current user by ID: {Id}", currentUserId);
        var response = await _mediator.Send(command);

        if (response.IsSuccess)
        {
            _logger.LogInformation("Current user with ID: {Id} successfully deleted", currentUserId);
            return Ok(response.Value);
        }

        _logger.LogError("Failed to delete current user with ID: {Id}. Reasons: {Reasons}", currentUserId, response.Reasons);
        return NotFound(response.Reasons);
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

        var command = new ConfirmUserEmailCommand
        {
            UserId = userId,
            ConfirmationToken = decodedToken
        };

        _logger.LogInformation("Confirming email for user by ID: {Id}", userId);
        var response = await _mediator.Send(command);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Email for user with ID: {Id} successfully confirmed", userId);
            return Ok("Mail confirmed");
        }

        _logger.LogError("Failed to confirm email for user with ID: {Id}. Reasons: {Reasons}", userId, response.Reasons);
        return NotFound(response.Reasons);
    }
}
