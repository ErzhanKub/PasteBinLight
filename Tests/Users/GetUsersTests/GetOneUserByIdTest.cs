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
        private readonly Mock<ILogger<FetchUserByIdHandler>> _loggerMock;

        public GetOneUserByIdTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<FetchUserByIdHandler>>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var handler = new FetchUserByIdHandler(_userRepositoryMock.Object, _loggerMock.Object);
            var request = new FetchUserByIdRequest { UserId = userId };
            var user = new User(Guid.NewGuid(), new Username("username"), new Password("password12345"), new Email("test.user@example.com", true), Role.User, default);
            
            _userRepositoryMock.Setup(x => x.FetchByIdAsync(userId, default))
                .ReturnsAsync(user);

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.FetchByIdAsync(It.IsAny<Guid>(), default), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ShouldReturnFailure()
        {
            // Arrange
            var handler = new FetchUserByIdHandler(_userRepositoryMock.Object, _loggerMock.Object);

            _userRepositoryMock.Setup(x => x.FetchByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((User)null!);

            // Act
            var result = await handler.Handle(new FetchUserByIdRequest { UserId = Guid.NewGuid() }, default);

            // Assert
            result.IsFailed.Should().BeTrue();
            _userRepositoryMock.Verify(x => x.FetchByIdAsync(It.IsAny<Guid>(), default), Times.Once());
        }

    }
}
