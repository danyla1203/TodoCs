using todo.Data.Dto;
using todo.Models;

namespace todo.Data.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> FindUserByEmail(string email);
    Task<CreatedUserDto?> GetProfile(int id);
}