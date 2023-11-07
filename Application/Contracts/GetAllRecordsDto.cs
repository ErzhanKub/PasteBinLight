﻿namespace Application.Contracts;

public class GetAllRecordsDto
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
    public DateTime DateCreated { get; init; }
    public long Likes { get; init; }
    public long DisLikes { get; init; }
}