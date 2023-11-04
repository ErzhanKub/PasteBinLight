using Application.Features.Users.Delete;
using Application.Shared;
using Domain.Entities;
using Domain.Repositories;

namespace Tests.Users
{
    public class DeleteUserTest
    {

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public DeleteUserTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldDeleteUser()
        {
            // Arrange
            var command = new DeleteUsersByIdsCommand
            {
                Id = new Guid[] { Guid.NewGuid() }
            };
            var handler = new DeleteUsersByIdsHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);

            _userRepositoryMock.Setup(x => x.DeleteRangeAsync(It.IsAny<Guid[]>()))
                .ReturnsAsync(command.Id);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(command.Id);
            _userRepositoryMock.Verify(x => x.DeleteRangeAsync(It.IsAny<Guid[]>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveCommitAsync(), Times.Once);
        }
    }
}
