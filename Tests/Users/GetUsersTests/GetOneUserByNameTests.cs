using Application.Features.Users.Get;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Users.GetUsersTests
{
    public class GetOneUserByNameTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetOneUserRequestHandler>> _loggerMock;

        public GetOneUserByNameTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<GetOneUserRequestHandler>>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnUser()
        {
            // Arrange
            var userName = "user123";
            var handler = new GetUserByUsernameRequestHandler(_userRepositoryMock.Object, _loggerMock.Object);
            var request = new GetUserByUsernameRequest { Username = userName };
            var user = new User(Guid.NewGuid(), new Username("username"), new Password("password12345"), new Email("test.user@example.com", true), Role.User, default);

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(userName))
                .ReturnsAsync(user);

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.GetByUsernameAsync(userName), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldReturnFailure()
        {
            // Arrange
            var handler = new GetUserByUsernameRequestHandler(_userRepositoryMock.Object, _loggerMock.Object);

            _userRepositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null!);

            // Act
            var result = await handler.Handle(new GetUserByUsernameRequest { Username = null }, default);

            // Assert
            result.IsFailed.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.GetByUsernameAsync(It.IsAny<string>()), Times.Once());
        }
    }
}
