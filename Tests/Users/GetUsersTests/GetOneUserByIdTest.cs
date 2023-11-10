using Application.Features.Users.Get;
using Application.Features.Users.Update;
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
    public class GetOneUserByIdTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetOneUserRequestHandler>> _loggerMock;

        public GetOneUserByIdTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<GetOneUserRequestHandler>>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var handler = new GetOneUserRequestHandler(_userRepositoryMock.Object, _loggerMock.Object);
            var request = new GetOneUserRequest { Id = userId };
            var user = new User(Guid.NewGuid(), new Username("username"), new Password("password12345"), new Email("test.user@example.com", true), Role.User, default);
            
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldReturnFailure()
        {
            // Arrange
            var handler = new GetOneUserRequestHandler(_userRepositoryMock.Object, _loggerMock.Object);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null!);

            // Act
            var result = await handler.Handle(new GetOneUserRequest { Id = Guid.NewGuid() }, default);

            // Assert
            result.IsFailed.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());
        }

    }
}
