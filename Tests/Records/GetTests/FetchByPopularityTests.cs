using Application.Features.Records.Get;
using Application.Features.Records.Update;
using Application.Shared;
using Domain.IServices;
using Domain.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Records.GetTests
{
    public class FetchByPopularityRecordsTests
    {
        [Fact]
        public async Task Handle_WithRecords_ReturnsListOfAllRecordsDto()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var recordRepositoryMock = new Mock<IRecordRepository>();
            var loggerMock = new Mock<ILogger<FetchByPopularityHandler>>();

            var request = new FetchByPopularityRequest();

            var records = new List<Domain.Entities.Record>
            {
                new Domain.Entities.Record
                {
                    Id = Guid.NewGuid(),
                    Title = "Record 1",
                    Url = new Uri("http://example.com"),
                    DateCreated = DateTime.Now,
                    DeadLine = DateTime.Now.AddDays(7),
                    Likes = 100,
                    DisLikes = 20
                },
            };

            recordRepositoryMock.Setup(repo => repo.FetchTop100RecordsByLikesAsync(cancellationToken)).ReturnsAsync(records);

            var handler = new FetchByPopularityHandler(recordRepositoryMock.Object, loggerMock.Object);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(records.Count);

        }

        [Fact]
        public async Task Handle_WithNoRecords_ReturnsFailureResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var recordRepositoryMock = new Mock<IRecordRepository>();
            var loggerMock = new Mock<ILogger<FetchByPopularityHandler>>();

            var request = new FetchByPopularityRequest();

            recordRepositoryMock.Setup(repo => repo.FetchTop100RecordsByLikesAsync(cancellationToken)).ReturnsAsync((List<Domain.Entities.Record>)null);

            var handler = new FetchByPopularityHandler(recordRepositoryMock.Object, loggerMock.Object);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.IsFailed.Should().BeTrue();
        }
    }
}
