using Application.Features.Users.Create;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;

namespace Tests.Users;

public class CreateUserTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateUserTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUser()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Username = "Test",
            Email = "test.user@example.com",
            Password = "password123",
        };

        var handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveCommitAsync(), Times.Once);
    }
}
