using todo.Models;

namespace todo.Data.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<User?> FindUserByEmail(string email);
}