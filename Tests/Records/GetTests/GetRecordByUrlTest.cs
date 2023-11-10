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
    public class GetRecordByUrlTest
    {
        private readonly Mock<IRecordRepository> _recordRepositoryMock;
        private readonly Mock<ILogger<GetRecordByUrlHandler>> _loggerMock;
        private readonly Mock<IRecordCloudService> _recordCloudServiceMock;

        public GetRecordByUrlTest()
        {
            _recordRepositoryMock = new Mock<IRecordRepository>();
            _loggerMock = new Mock<ILogger<GetRecordByUrlHandler>>();
            _recordCloudServiceMock = new Mock<IRecordCloudService>();
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldReturnRecordDto()
        {
            //Arrange
            var recordId = Guid.NewGuid();
            var encodedGuid = "valid-encoded-guid";
            var userId = Guid.NewGuid();

            var request = new GetRecordByUrlRequest
            {
                EncodedGuid = encodedGuid,
                UserId = userId
            };

            var record = new Domain.Entities.Record()
            {

                Id = recordId,
                Url = new Uri("http://example.com/record1")

            };

            var expectedRecordDto = new RecordDto
            {
                Text = "qwertyqwerty",
                Id = record.Id,

            };

            _recordRepositoryMock.Setup(repo => repo.DecodeGuidFromBase64(encodedGuid)).Returns(recordId);
            _recordRepositoryMock.Setup(repo => repo.FetchByIdAsync(recordId, It.IsAny<CancellationToken>())).ReturnsAsync(record);
            _recordCloudServiceMock.Setup(service => service.FetchTextFileFromCloudAsync(record.Url)).ReturnsAsync("Text content");

            var handler = new GetRecordByUrlHandler(_recordRepositoryMock.Object, _loggerMock.Object, _recordCloudServiceMock.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedRecordDto.Id, result.Value.Id);

        }
        [Fact]
        public void GetRecordByUrlRequest_WithInvalidModel_ShouldFailValidation()
        {
            // Arrange
            var validator = new GetRecordByUrlRequestValidator();
            var invalidRequest = new GetRecordByUrlRequest();

            // Act
            var result = validator.Validate(invalidRequest);

            // Assert
            Assert.False(result.IsValid);
            Assert.True(result.Errors.Count > 0);
            Assert.Contains(result.Errors, error => error.PropertyName == "EncodedGuid");
        }

        [Fact]
        public void GetRecordByUrlRequest_WithValidModel_ShouldPassValidation()
        {
            // Arrange
            var validator = new GetRecordByUrlRequestValidator();
            var validRequest = new GetRecordByUrlRequest
            {
                EncodedGuid = "valid-encoded-guid",
                UserId = Guid.NewGuid()
            };

            // Act
            var result = validator.Validate(validRequest);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

    }

}

