﻿using Application.Contracts;
using Application.Features.Records.Create;
using Application.Features.Users.Update;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Mapster;
using Tests.TheoryDataObjects;

namespace Tests.Users
{
    public class UpdateUserTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateUserByIdCommandHandler>> _loggerMock;

        public UpdateUserTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateUserByIdCommandHandler>>();
        }

        [Theory]
        [ClassData(typeof(UserDtoData))]
        public async Task Handle_ValidCommand_ShouldUpdateUser(UserDto dto)
        {
            // Arrange
            var user = new User(Guid.NewGuid(), new Username("username"), new Password("password12345"), new Email("test.user@example.com", true), Role.User, default);
            _userRepositoryMock.Setup(x => x.FetchByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(user);
            var handler = new UpdateUserByIdCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
            var command = new UpdateUserByIdCommand
            {
                UserId = user.Id,
                Data = dto.Adapt<UpdateUserDto>(),
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveAndCommitAsync(default), Times.Once);
        }


        [Theory]
        [ClassData(typeof(UserDtoData))]
        public async Task Handle_InvalidUser_ShouldReturnFailureResult(UserDto dto)
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.FetchByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((User)null);
            var handler = new UpdateUserByIdCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
            var command = new UpdateUserByIdCommand
            {
                UserId = Guid.NewGuid(),
                Data = dto.Adapt<UpdateUserDto>(),
              
            };
            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveAndCommitAsync(default), Times.Never);
        }

    }
}