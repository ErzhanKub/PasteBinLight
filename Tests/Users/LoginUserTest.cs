using Application.Features.Users.Login;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;

namespace Tests.Users
{
    public class LoginUserTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;

        public LoginUserTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "test.user@example.com",
                Password = "password123",
            };
            var handler = new LoginHandler(_configurationMock.Object, _userRepositoryMock.Object);

            _userRepositoryMock.Setup(x => x.CheckUserCredentialsAsync(request.Username, request.Password))
                .ReturnsAsync(new User { Username = request.Username, Password = request.Password, Email = "qwerty" });
            _configurationMock.Setup(x => x["Jwt"]).Returns("qwertyuiopasdfgh");


            // Act
            var result = await handler.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNullOrEmpty();
            _userRepositoryMock.Verify(x => x.CheckUserCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
