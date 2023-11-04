﻿using Application.Contracts;
using Application.Shared;
using Domain.Enums;
using Domain.Repositories;

namespace Application.Features.Users.Update;

public record UpdateUserByIdCommand : IRequest<Result<UpdateUserByIdDto>>
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? Email { get; init; }
    public int UserRole { get; init; }
}

public class UpdateUserByIdValidator : AbstractValidator<UpdateUserByIdCommand>
{
    public UpdateUserByIdValidator()
    {
        RuleFor(u => u.Username).Length(2, 200);
        RuleFor(p => p.Password).Length(5, 200);
        RuleFor(e => e.Email).EmailAddress();
        RuleFor(r => r.UserRole).ExclusiveBetween(0, 3);
    }
}

public class UpdateUserByIdCommandHandler : IRequestHandler<UpdateUserByIdCommand, Result<UpdateUserByIdDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;

    public UpdateUserByIdCommandHandler(IUserRepository userRepository, IUnitOfWork uow)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<UpdateUserByIdDto>> Handle(UpdateUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
            return Result.Fail("User is not found");

        if (request.Username is not null)
            user.UpdateUsername(request.Username);

        if (request.Password is not null)
            user.UpdatePassword(request.Password);

        if (request.Email is not null)
            user.UpdateEmail(request.Email);

        user.UpdateRole((Role)request.UserRole);

        _userRepository.Update(user);

        await _uow.SaveCommitAsync();

        var response = new UpdateUserByIdDto
        {
            Email = user.Email.Value,
            Id = user.Id,
            Username = user.Username.Value,
            Role = user.Role,
        };

        return Result.Ok(response);
    }

}