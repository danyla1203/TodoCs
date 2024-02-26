using todo.Data.Dto;
using todo.Data.Mappings;
using todo.Exceptions;
using todo.Models;

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
        User existingUser = await _unit.UserRepository.FindUserByEmail(user.Email);
        if (existingUser != null) throw new UserAlreadyExist();
        var newRecord = await _unit.UserRepository.AddItem(UserMapping.ToEntity(user));
        return UserMapping.ToPublicData(newRecord);
    }

    public async Task<CreatedUserDto> GetUser(int id)
    {
        User? record = await _unit.UserRepository.GetById(id);
        if (record == null) throw new UserNotFound();
        return UserMapping.ToPublicData(record);
    }
}