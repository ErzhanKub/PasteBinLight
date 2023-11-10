using Application.Contracts;
using Application.Features.Records.Get;
using Domain.IServices;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Records.GetTests
{
    public class RecordByTitleTests
    {
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<ILogger<RecordByTitleHandler>> _loggerMock;
        public RecordByTitleTests()
        {
            _recordRepositoryMock = new Mock<IRecordRepository>();
            _loggerMock = new Mock<ILogger<RecordByTitleHandler>>();
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldReturnListOfRecordsDto()
        {
            // Arrange
            var recordTitle = "Sample Record";
            var cancellationToken = CancellationToken.None;

            var request = new RecordByTitleRequest
            {
                RecordTitle = recordTitle
            };

            var records = new List<Domain.Entities.Record>
            {
                new Domain.Entities.Record
                {
                    Id = Guid.NewGuid(),
                    Title = recordTitle,
                    Url = new Uri("http://example.com/record1")
                },
            };

            var expectedRecordsDto = records.Select(record => new AllRecordsDto
            {
                Id = record.Id,
                Title = record.Title,
                Url = record.Url,
                DateCreated = record.DateCreated,
                DeadLine = record.DeadLine,
                Likes = record.Likes,
                DisLikes = record.DisLikes,
            }).ToList();

            _recordRepositoryMock.Setup(repo => repo.FindRecordsByTitleAsync(recordTitle, cancellationToken)).ReturnsAsync(records);

            var handler = new RecordByTitleHandler(_recordRepositoryMock.Object, _loggerMock.Object);

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedRecordsDto.Count, result.Value.Count);
        }
        [Fact]
        public void RecordByTitleRequest_WithInvalidModel_ShouldFailValidation()
        {
            // Arrange
            var validator = new RecordByTitleValidator();
            var invalidRequest = new RecordByTitleRequest
            {
                RecordTitle = null
            }; 

            // Act
            var result = validator.Validate(invalidRequest);

            // Assert
            Assert.False(result.IsValid);
            Assert.True(result.Errors.Count > 0);
            Assert.Contains(result.Errors, error => error.PropertyName == "RecordTitle");
        }

        [Fact]
        public void RecordByTitleRequest_WithValidModel_ShouldPassValidation()
        {
            // Arrange
            var validator = new RecordByTitleValidator();
            var validRequest = new RecordByTitleRequest
            {
                RecordTitle = "Sample Record"
            };

            // Act
            var result = validator.Validate(validRequest);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
