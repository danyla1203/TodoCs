using todo.Data.Dto;
using todo.Models;

namespace todo.Data.Mappings;

public class UserMapping
{
    public static User ToEntity(AddUserDto input)
    {
        return new User
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = input.Password
        };
    }
    public static CreatedUserDto ToPublicData(User user)
    {
        return new CreatedUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
        };
    }
}