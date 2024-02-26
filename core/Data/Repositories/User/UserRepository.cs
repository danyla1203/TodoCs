using todo.Models;

namespace todo.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
  public UserRepository(ApplicationDbContext context)
        : base(context)
  {}

    public Task<User> FindUserByEmail(string email)
    {
        throw new NotImplementedException();
    }
}