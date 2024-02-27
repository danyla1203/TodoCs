using Microsoft.EntityFrameworkCore;
using todo.Models;

namespace todo.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context)
          : base(context)
    { }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await _table.FirstOrDefaultAsync(user => user.Email == email);
    }

    public override async Task<User?> GetById(int id)
    {
        return await _table
            .Include(user => user.Tasks)
            .SingleOrDefaultAsync(user => user.Id == id);
    }
}