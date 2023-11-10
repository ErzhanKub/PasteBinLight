using Application.Features.Records.Get;
using Domain.IServices;
using Domain.Repositories;

namespace Tests.Records.GetTests
{
    public class GetRecordByIdHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidRecordIdAndUserId_ReturnsRecordDto()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var recordId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var recordRepositoryMock = new Mock<IRecordRepository>();
            var recordCloudServiceMock = new Mock<IRecordCloudService>();
            var loggerMock = new Mock<ILogger<GetRecordByIdHandler>>();

            var request = new GetRecordByIdRequest
            {
                RecordId = recordId,
                UserId = userId
            };

            var record = new Domain.Entities.Record
            {
                Id = recordId,
                Title = "Test Record",
                Url = new Uri("http://example.com"),
                DateCreated = DateTime.Now,
                DeadLine = DateTime.Now.AddDays(7),
                IsPrivate = false,
                Likes = 10,
                DisLikes = 2
            };

            recordRepositoryMock.Setup(repo => repo.FetchByIdAsync(recordId, cancellationToken)).ReturnsAsync(record);
            recordCloudServiceMock.Setup(service => service.FetchTextFileFromCloudAsync(record.Url)).ReturnsAsync("Test Text");

            var handler = new GetRecordByIdHandler(recordRepositoryMock.Object, loggerMock.Object, recordCloudServiceMock.Object);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            var recordDto = result.Value;
            recordDto.Id.Should().Be(record.Id);
            recordDto.Title.Should().Be(record.Title);
            recordDto.Text.Should().Be("Test Text");
            recordDto.DateCreated.Should().Be(record.DateCreated);
            recordDto.DeadLine.Should().Be(record.DeadLine);
            recordDto.IsPrivate.Should().Be(record.IsPrivate);
            recordDto.Likes.Should().Be(record.Likes);
            recordDto.DisLikes.Should().Be(record.DisLikes);
        }

        [Fact]
        public async Task Handle_WithInvalidRecordId_ReturnsFailureResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var recordId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var recordRepositoryMock = new Mock<IRecordRepository>();
            var loggerMock = new Mock<ILogger<GetRecordByIdHandler>>();

            var request = new GetRecordByIdRequest
            {
                RecordId = recordId,
                UserId = userId
            };

            recordRepositoryMock.Setup(repo => repo.FetchByIdAsync(recordId, cancellationToken)).ReturnsAsync((Domain.Entities.Record)null);

            var handler = new GetRecordByIdHandler(recordRepositoryMock.Object, loggerMock.Object, Mock.Of<IRecordCloudService>());

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WithAccessDenied_ReturnsFailureResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var recordId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var recordRepositoryMock = new Mock<IRecordRepository>();
            var loggerMock = new Mock<ILogger<GetRecordByIdHandler>>();

            var request = new GetRecordByIdRequest
            {
                RecordId = recordId,
                UserId = userId
            };

            var record = new Domain.Entities.Record
            {
                Id = recordId,
                Title = "Test Record",
                Url = new Uri("http://example.com"),
                DateCreated = DateTime.Now,
                DeadLine = DateTime.Now.AddDays(7),
                IsPrivate = true,
                UserId = Guid.NewGuid() // A different user ID
            };

            recordRepositoryMock.Setup(repo => repo.FetchByIdAsync(recordId, cancellationToken)).ReturnsAsync(record);

            var handler = new GetRecordByIdHandler(recordRepositoryMock.Object, loggerMock.Object, Mock.Of<IRecordCloudService>());

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
        }
    }
}
