using Application.Features.Users.Get;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Users
{
    public class GetAllUsersTest
    {
        public class GetAllEmployeeHandlerTests
        {
            private readonly Mock<IUserRepository> _repositoryMock;

            public GetAllEmployeeHandlerTests()
            {
                _repositoryMock = new Mock<IUserRepository>();
            }

            [Fact]
            public async Task Handle_ValidRequest_ShouldReturnAllEmployees()
            {
                //Arrange
                var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "username",
                    Email = "test.user@example.com",
                    Password = "qeqrqwr",
                    Postes = new List<Poste>
                    {
                        new Poste
                        {
                            Name = "Test",
                            Comment= "Test",
                            Priority= 10,
                            Status = StatusTask.Done,
                            Project = new Project(),
                            ExecutorId= Guid.NewGuid(),
                        }
                    },
                    AuthoredTasks = new List<CustomTask>(),
                    ManagedProjects = new List<Project>(),
                    MemberProjects= new List<Project>(),
                    Role = Role.Employee
                },
                new Employee
                { Id = Guid.NewGuid(),
                    Firstname = "Erzhan",
                    Lastname = "Kub",
                    Email = "john.doe@example.com",
                    Role = Role.ProjectManager
                }
            };
                var handler = new GetAllRequestHandler(_repositoryMock.Object);
                _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(employees);

                //Act
                var result = await handler.Handle(new GetAllRequest(), default);

                //Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeEquivalentTo(users, opts => opts.ExcludingMissingMembers());
                _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
            }

            [Fact]
            public async Task Handle_InvalidRequest_ShouldReturnFailure()
            {
                // Arrange
                var handler = new GetAllRequestHandler(_repositoryMock.Object);
                _repositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync((List<User>)null!);

                // Act
                var result = await handler.Handle(new GetAllEmployeeRequest(), default);

                // Assert
                result.IsSuccess.Should().BeTrue();
                result.Value.Should().BeNull();
                _repositoryMock.Verify(x => x.GetAllAsync(), Times.Once());
            }
        }
}
