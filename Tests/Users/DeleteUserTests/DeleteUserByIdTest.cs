using Application.Features.Users.Delete;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;

namespace Tests.Users.DeleteUserTests
{
    public class DeleteUserByIdTest
    {

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DeleteUsersByIdsHandler>> _loggerMock;

        public DeleteUserByIdTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DeleteUsersByIdsHandler>>();
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldDeleteUser()
        {
            // Arrange
            var command = new DeleteUsersByIdsCommand
            {
                UserIds = new Guid[] { Guid.NewGuid() }
            };
            var handler = new DeleteUsersByIdsHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);

            _userRepositoryMock.Setup(x => x.RemoveByIdAsync(It.IsAny<Guid[]>()))
                .ReturnsAsync(command.UserIds);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(command.UserIds);
            _userRepositoryMock.Verify(x => x.RemoveByIdAsync(It.IsAny<Guid[]>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveAndCommitAsync(default), Times.Once);
        }
    }
}
