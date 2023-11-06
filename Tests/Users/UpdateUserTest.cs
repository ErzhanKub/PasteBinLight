using Application.Contracts;
using Application.Features.Users.Update;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Users
{
    public class UpdateUserTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UpdateUserTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldUpdateEmployee()
        {
            // Arrange
            var command = new UpdateUserByIdDto
            {
                    Id = Guid.NewGuid(),
                    Username = "Test",
                    Email = "test.user@example.com",
                    Role = Role.User,
             
            };
            var handler = new UpdateUserByIdCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User { Id = command.UserDto.Id });

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(command.UserDto, options => options.ExcludingMissingMembers());
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveCommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ShouldReturnFailure()
        {
            // Arrange
            var command = new UpdateUserByIdCommand
            {
                UserDto = new UpdateUserByIdDto
                {
                    Id = Guid.NewGuid(),
                    Username = "Test",
                    Email = "test.user@example.com",
                    Role = Role.User,
                }
            };
            var handler = new UpdateUserByIdCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null!);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _userRepositoryMock.Verify(x => x.Update(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveCommitAsync(), Times.Never);
        }
    }
}
