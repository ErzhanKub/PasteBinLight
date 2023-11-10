using Application.Features.Users.Create;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;


namespace Tests.Users;

public class CreateUserTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock;

    public CreateUserTest()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateUserCommandHandler>>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateUser()
    {
        var test = _userRepositoryMock.Object.HashPassword("password123");
        var a = new Password("password123");
        var b = a.Value;

        // Arrange
        var command = new CreateUserCommand
        {
            Username = "Test",
            Email = "test.user@example.com",
            Password = b
        };

        var handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), default), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveAndCommitAsync(default), Times.Once);
    }
}
