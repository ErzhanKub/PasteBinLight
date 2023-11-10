using Application.Features.Users.Delete;
using Application.Features.Users.Update;
using Application.Shared;
using Domain.Repositories;
using FluentResults;

namespace Tests.Users.DeleteUserTests
{
    public class DeleteByUsernameCommadHandlerTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DeleteByUsernameCommandHandler>> _loggerMock;

        public DeleteByUsernameCommadHandlerTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteByUsernameCommandHandler>>();
        }

        [Fact]
        public async Task Handle_ValidUsername_DeletesUserAndReturnsResult()
        {
            // Arrange
            var usernameToDelete = "userToDelete";
            var handler = new DeleteByUsernameCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
            var command = new DeleteByUsernameCommand { TargetUsername = usernameToDelete };

            _userRepositoryMock.Setup(x => x.RemoveUserByUsernameAsync(usernameToDelete))
                .ReturnsAsync(command.TargetUsername);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(command.TargetUsername);
            _unitOfWorkMock.Verify(x => x.SaveAndCommitAsync(default), Times.Once);

            // Verify that DeleteUserByUsernameAsync was called with the correct username
            _userRepositoryMock.Verify(x => x.RemoveUserByUsernameAsync(usernameToDelete), Times.Once);


        }

        
    }
}
