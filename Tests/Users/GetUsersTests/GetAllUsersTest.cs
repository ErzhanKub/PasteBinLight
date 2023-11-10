using Application.Contracts;
using Application.Features.Users.Get;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using FluentResults;
using System.Threading;

namespace Tests.Users.GetUsersTests
{
    public class GetAllUsersTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<GetAllUsersHandler>> _loggerMock;

        public GetAllUsersTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<GetAllUsersHandler>>();
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldReturnAllUsers()
        {
            //Arrange
            var handler = new GetAllUsersHandler(_userRepositoryMock.Object, _loggerMock.Object);
            var requset = new GetAllUsersRequest();


            var users = new List<User>
            {
            new User(Guid.NewGuid(), new Username("user1"), new Password("password12345"), new Email("user1@example.com", true), Role.User, default),
            new User(Guid.NewGuid(), new Username("user2"), new Password("password54321"), new Email("user2@example.com", true), Role.User, default)
            };

            _userRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), default)).ReturnsAsync(users);

            //Act
            var result = await handler.Handle(requset, default);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(users, opts => opts.ExcludingMissingMembers());
            _userRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), default), Times.Once());
        }

    }
}
