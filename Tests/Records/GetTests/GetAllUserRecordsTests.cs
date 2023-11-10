using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Features.Records.Get;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace Tests.Records.GetTests
{
    public class GetAllUserRecordsTests
    {
        [Fact]
        public async Task Handle_WithValidUserId_ReturnsRecordsList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cancellationToken = CancellationToken.None;

            var userRepositoryMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<GetAllUserRecordsHandler>>();

            var user = new User
            {
                Id = userId,
                
            };
            var record = new Domain.Entities.Record { Id = userId, Title = "Record 1", Url = new Uri("http://example.comn") };

            user.AddRecordToUser(record);

            userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, cancellationToken)).ReturnsAsync(user);

            var handler = new GetAllUserRecordsHandler(userRepositoryMock.Object, loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetAllUserRecordsRequest { UserId = userId }, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2); // Assuming there are two records in the user's list
        }


        [Fact]
        public async Task Handle_WithInvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var cancellationToken = CancellationToken.None;

            var userRepositoryMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<GetAllUserRecordsHandler>>();

            userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, cancellationToken)).ReturnsAsync((User)null);

            var handler = new GetAllUserRecordsHandler(userRepositoryMock.Object, loggerMock.Object);

            // Act
            var result = await handler.Handle(new GetAllUserRecordsRequest { UserId = userId }, cancellationToken);

            // Assert
            result.IsFailed.Should().BeTrue();
        }

    }

}
