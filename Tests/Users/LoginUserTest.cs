using Application.Contracts;
using Application.Features.Users.Login;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Users
{
    public class LoginUserTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<UserLoginHandler>> _loggerMock;

        public LoginUserTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<UserLoginHandler>>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnToken()
        {

            _userRepositoryMock.Setup(x => x.ValidateUserCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()));

            // Arrange
            var request = new UserLoginRequest
            {
                Username = "test.user@example.com",
                Password = "password123",
            };
            var handler = new UserLoginHandler(_configurationMock.Object, _userRepositoryMock.Object, _loggerMock.Object);
            _configurationMock.Setup(x => x["Jwt"]).Returns("qwertyuiopasdfgh");
            // Act
            var result = await handler.Handle(request, default);

            // Assert

            _userRepositoryMock.Verify(x => x.ValidateUserCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
