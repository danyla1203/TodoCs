using todo.Data.Dto;

namespace todo.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unit;
    public UserService(IUnitOfWork unit)
    {
        _unit = unit;
    }
    public async Task<CreatedUserDto> CreateUser(AddUserDto user)
    {
       throw new NotImplementedException();
    }

    public async Task<CreatedUserDto> GetUser(int id)
    {
        throw new NotImplementedException();
    }
}