using Application.Features.Records.Create;
using Application.Features.Records.Delete;
using Application.Shared;
using Domain.Entities;
using Domain.IServices;
using Domain.Repositories;
using FluentValidation.TestHelper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Records
{
    public class DeleteRecordByIdHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidUserIdAndRecordId_DeletesRecordAndReturnsRecordId()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = Guid.NewGuid();
            var recordId = Guid.NewGuid();

            var userRepositoryMock = new Mock<IUserRepository>();
            var recordRepositoryMock = new Mock<IRecordRepository>();
            var recordCloudServiceMock = new Mock<IRecordCloudService>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<DeleteRecordByIdHandler>>();

            var request = new DeleteRecordByIdCommand
            {
                UserId = userId,
                RecordId = recordId
            };

            var user = new Domain.Entities.User
            {
                Id = userId,
            };
            var record = new Domain.Entities.Record { Id = Guid.NewGuid(), Url = new Uri("http://example.com") };
            user.AddRecordToUser(record);

            userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, cancellationToken)).ReturnsAsync(user);
            recordRepositoryMock.Setup(repo => repo.RemoveByIdAsync(recordId)).ReturnsAsync(Array.Empty<Guid>());

            var handler = new DeleteRecordByIdHandler(userRepositoryMock.Object, recordRepositoryMock.Object, unitOfWorkMock.Object, loggerMock.Object, recordCloudServiceMock.Object);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(recordId);

            // Ensure DeleteTextFileFromCloudAsync is called
            recordCloudServiceMock.Verify(service => service.DeleteTextFileFromCloudAsync(recordId.ToString()), Times.Once);

            // Ensure RemoveByIdAsync is called
            recordRepositoryMock.Verify(repo => repo.RemoveByIdAsync(recordId), Times.Once);

            // Ensure Update, SaveAndCommitAsync are called
            userRepositoryMock.Verify(repo => repo.Update(user), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveAndCommitAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidUserId_ReturnsFailureResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = Guid.NewGuid();
            var recordId = Guid.NewGuid();

            var userRepositoryMock = new Mock<IUserRepository>();
            var loggerMock = new Mock<ILogger<DeleteRecordByIdHandler>>();

            var request = new DeleteRecordByIdCommand
            {
                UserId = userId,
                RecordId = recordId
            };

            userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, cancellationToken)).ReturnsAsync((Domain.Entities.User)null);

            var handler = new DeleteRecordByIdHandler(userRepositoryMock.Object, Mock.Of<IRecordRepository>(), Mock.Of<IUnitOfWork>(), loggerMock.Object, Mock.Of<IRecordCloudService>());

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WithInvalidRecordId_ReturnsFailureResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = Guid.NewGuid();
            var recordId = Guid.NewGuid();

            var userRepositoryMock = new Mock<IUserRepository>();
            var recordRepositoryMock = new Mock<IRecordRepository>();
            var loggerMock = new Mock<ILogger<DeleteRecordByIdHandler>>();

            var request = new DeleteRecordByIdCommand
            {
                UserId = userId,
                RecordId = recordId
            };

            var user = new Domain.Entities.User
            {
                Id = userId,

            };
            var record = new Domain.Entities.Record { Id = Guid.NewGuid(), Url = new Uri("http://example.com") };
            user.AddRecordToUser(record);

            userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, cancellationToken)).ReturnsAsync(user);

            var handler = new DeleteRecordByIdHandler(userRepositoryMock.Object, recordRepositoryMock.Object, Mock.Of<IUnitOfWork>(), loggerMock.Object, Mock.Of<IRecordCloudService>());

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
        }
    }
}
