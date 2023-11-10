using Application.Features.Records.Create;
using Application.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.IServices;
using Domain.Repositories;
using FluentResults;
using Record = Domain.Entities.Record;

namespace Tests.Records
{
    public class CreateRecordTest
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<IRecordCloudService> _recordCloudServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateRecordHandler>> _loggerMock;

        public CreateRecordTest()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _recordRepositoryMock = new Mock<IRecordRepository>();
            _recordCloudServiceMock = new Mock<IRecordCloudService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateRecordHandler>>();
        }
    

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateRecord()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var text = "Sample text for the record";
            var title = "Record Title";
            var deadline = DateTime.Now.AddDays(7);
            var isPrivate = false;
            var handler = new CreateRecordHandler(_userRepositoryMock.Object, _recordRepositoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object, _recordCloudServiceMock.Object);
            var command = new CreateRecordCommand
            {
                UserId = userId,
                Text = text,
                Title = title,
                DeadLine = deadline,
                IsPrivate = isPrivate,

            };
            var user = new User(Guid.NewGuid(), new Username("username"), new Password("password12345"), new Email("test.user@example.com", true), Role.User, default);

            _userRepositoryMock.Setup(repo => repo.FetchByIdAsync(userId, default)).ReturnsAsync(user);
            _recordCloudServiceMock.Setup(service => service.UploadTextFileToCloudAsync(It.IsAny<string>(), text)).ReturnsAsync("sampleUrl");
            _recordRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Record>(), default)).ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<string>>();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            _userRepositoryMock.Verify(repo => repo.FetchByIdAsync(userId, default), Times.Once);

            _recordCloudServiceMock.Verify(service => service.UploadTextFileToCloudAsync(It.IsAny<string>(), text), Times.Once);

            _recordRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Record>(), default), Times.Once);

            _userRepositoryMock.Verify(repo => repo.Update(It.IsAny<User>()), Times.Once);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveAndCommitAsync(default), Times.Once);

            _loggerMock.Verify(logger => logger.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(3));
        }

    }
}