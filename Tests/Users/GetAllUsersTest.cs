//using Application.Contracts;
//using Application.Features.Users.Get;
//using Domain.Entities;
//using Domain.Enums;
//using Domain.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Tests.Users
//{
//    public class GetAllUsersTest
//    {
//        private readonly Mock<IUserRepository> _repositoryMock;
//        private readonly Mock<ILogger<GetAllRequestHandler>> _loggerMock;

//        public GetAllUsersTest()
//        {
//            _repositoryMock = new Mock<IEmployeeRepository>();
//            _loggerMock = new Mock<ILogger<GetAllRequestHandler>>();
//        }

//        [Fact]
//        public async Task Handle_ValidRequest_ShouldReturnAllUsers()
//        {
//            //Arrange
//            var users = new List<User>
//            {
//                new UserDto
//                {
//                    Id = Guid.NewGuid(),
//                    Username = "Test",
//                    Email = "User",
//                    Role = 1,
//                },
//                new UserDto
//                { 
//                    Id = Guid.NewGuid(),
//                    Username = "Erzhan",
//                    Email = "john.doe@example.com",
//                    Role = 1
//                }
//            };
//            var handler = new GetAllEmployeeHandler(_repositoryMock.Object);
//            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(employees);

//            //Act
//            var result = await handler.Handle(new GetAllEmployeeRequest(), default);

//            //Assert
//            result.IsSuccess.Should().BeTrue();
//            result.Value.Should().BeEquivalentTo(employees, opts => opts.ExcludingMissingMembers());
//            _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
//        }

//        [Fact]
//        public async Task Handle_InvalidRequest_ShouldReturnFailure()
//        {
//            // Arrange
//            var handler = new GetAllEmployeeHandler(_repositoryMock.Object);
//            _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync((List<Employee>)null!);

//            // Act
//            var result = await handler.Handle(new GetAllEmployeeRequest(), default);

//            // Assert
//            result.IsSuccess.Should().BeTrue();
//            result.Value.Should().BeNull();
//            _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
//        }
//    }
//}
