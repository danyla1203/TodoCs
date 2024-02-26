using todo.Data.Dto;

namespace todo.Services;

public interface IUserService
{
    Task<CreatedUserDto> CreateUser(AddUserDto user);
    Task<CreatedUserDto> GetUser(int id);
}