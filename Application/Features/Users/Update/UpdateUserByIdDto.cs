﻿using Domain.Enums;

namespace Application.Features.Users.Update;

public record UpdateUserByIdDto
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public Role Role { get; init; }

}
