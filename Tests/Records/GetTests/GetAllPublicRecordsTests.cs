using Application.Features.Records.Get;
using Domain.Repositories;

namespace Tests.Records.GetTests
{
    public class GetAllPublicRecordsHandlerTests
    {
        [Fact]
        public async Task Handle_WithValidRequest_ShouldReturnListOfPublicRecordsDto()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            var recordRepositoryMock = new Mock<IRecordRepository>();
            var loggerMock = new Mock<ILogger<GetAllPublicRecordsHandler>>();

            var request = new GetAllPublicRecordsRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var allRecords = new List<Domain.Entities.Record>
            {
                new Domain.Entities.Record
                {
                    Id = Guid.NewGuid(),
                    IsPrivate = false,
                    Url = new Uri("http://example.com/record1")

                },
            };

            recordRepositoryMock.Setup(repo => repo.GetAllAsync(pageNumber, pageSize, cancellationToken)).ReturnsAsync(allRecords);

            var handler = new GetAllPublicRecordsHandler(recordRepositoryMock.Object, loggerMock.Object);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(allRecords.Count, result.Value.Count);
        }

        [Fact]
        public void GetAllPublicRecordsRequest_WithInvalidModel_ShouldFailValidation()
        {
            // Arrange
            var validator = new GetAllPublicRecordsRequestValidator();
            var invalidRequest = new GetAllPublicRecordsRequest
            {
                PageNumber = 0, // Invalid value
                PageSize = 0    // Invalid value
            };

            // Act
            var result = validator.Validate(invalidRequest);

            // Assert
            Assert.False(result.IsValid);
            Assert.True(result.Errors.Count == 2); 
        }

        [Fact]
        public void GetAllPublicRecordsRequest_WithValidModel_ShouldPassValidation()
        {
            // Arrange
            var validator = new GetAllPublicRecordsRequestValidator();
            var validRequest = new GetAllPublicRecordsRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = validator.Validate(validRequest);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
