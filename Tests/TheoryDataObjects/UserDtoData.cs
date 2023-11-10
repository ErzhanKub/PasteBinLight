using Application.Contracts;
using Domain.Enums;

namespace Tests.TheoryDataObjects;

public class UserDtoData : TheoryData<UserDto>
{
    public UserDtoData()
    {
        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Username = "newPassword",
            Email = "new@example.com",
            Role = Role.Admin,


        };

        Add(user);
    }
}
