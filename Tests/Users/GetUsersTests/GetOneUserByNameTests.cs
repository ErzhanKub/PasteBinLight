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
        private readonly Mock<ILogger<FetchUserByUsernameHandler>> _loggerMock;

        public GetOneUserByNameTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<FetchUserByUsernameHandler>>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnUser()
        {
            // Arrange
            var userName = "user123";
            var handler = new FetchUserByUsernameHandler(_userRepositoryMock.Object, _loggerMock.Object);
            var request = new FetchUserByUsernameRequest { TargetUsername = userName };
            var user = new User(Guid.NewGuid(), new Username("username"), new Password("password12345"), new Email("test.user@example.com", true), Role.User, default);

            _userRepositoryMock.Setup(x => x.FetchUserByUsernameAsync(userName, default))
                .ReturnsAsync(user);

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.FetchUserByUsernameAsync(userName, default), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldReturnFailure()
        {
            // Arrange
            var handler = new FetchUserByUsernameHandler(_userRepositoryMock.Object, _loggerMock.Object);

            _userRepositoryMock.Setup(x => x.FetchUserByUsernameAsync(It.IsAny<string>(), default))
                .ReturnsAsync((User)null!);

            // Act
            var result = await handler.Handle(new FetchUserByUsernameRequest { TargetUsername = null }, default);

            // Assert
            result.IsFailed.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.FetchUserByUsernameAsync(It.IsAny<string>(), default), Times.Once());
        }
    }
}
