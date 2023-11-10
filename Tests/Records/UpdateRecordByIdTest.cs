﻿using Application.Contracts;
using Application.Features.Records.Create;
using Application.Features.Records.Delete;
using Application.Features.Records.Update;
using Application.Shared;
using Domain.Entities;
using Domain.IServices;
using Domain.Repositories;
using Mapster;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.TheoryDataObjects;

namespace Tests.Records
{
    public class UpdateRecordByIdTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<IRecordCloudService> _recordCloudServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateRecordByIdHandler>> _loggerMock;

        public UpdateRecordByIdTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _recordRepositoryMock = new Mock<IRecordRepository>();
            _recordCloudServiceMock = new Mock<IRecordCloudService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateRecordByIdHandler>>();
        }

        [Theory]
        [ClassData(typeof(RecordDtoData))]
        [ClassData(typeof(UserDtoData))]
        public async Task Handle_WithValidCommand_ReturnsSuccessResult(RecordDto dto)
        {
            // Arrange
            var userId = Guid.NewGuid();

            var command = new UpdateRecordByIdCommand
            {
                UserId = userId,
                Data = dto.Adapt<UpdateRecordDto>(),
            };
            _userRepositoryMock.Setup(repo => repo.FetchByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(new User { });
            _recordRepositoryMock.Setup(p => p.FetchByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(new Domain.Entities.Record { Id = command.Data.RecordId, Url = new Uri("http://example.com") });

            // Set up the SaveAndCommitAsync to throw an exception
            _unitOfWorkMock.Setup(uow => uow.SaveAndCommitAsync(default)).ThrowsAsync(new Exception("Simulated error during save"));

            var handler = new UpdateRecordByIdHandler(
                _recordRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _loggerMock.Object,
                _recordCloudServiceMock.Object
            );

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            // Ensure SaveAndCommitAsync() is called
            _unitOfWorkMock.Verify(uow => uow.SaveAndCommitAsync(default), Times.Once);

            // Log the exception message
            _loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [ClassData(typeof(RecordDtoData))]
        public async Task Handle_WithInvalidUserId_ReturnsFailureResult(RecordDto dto)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var recordId = Guid.NewGuid();

            var command = new UpdateRecordByIdCommand
            {
                UserId = userId,
                Data = dto.Adapt<UpdateRecordDto>(),

            };

            var userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, default)).ReturnsAsync((User)null);

            var handler = new UpdateRecordByIdHandler(
                new Mock<IRecordRepository>().Object,
                new Mock<IUnitOfWork>().Object,
                userRepositoryMock.Object,
                new Mock<ILogger<UpdateRecordByIdHandler>>().Object,
                new Mock<IRecordCloudService>().Object
            );

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsFailed.Should().BeTrue();
        }
    }
}