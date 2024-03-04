using Microsoft.EntityFrameworkCore;
using todo.Data.Dto;
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

    public Task<CreatedUserDto?> GetProfile(int id)
    {
        return _table
            .Include(user => user.Tasks)
            .Where(user => user.Id == id)
            .Select(user => new CreatedUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Tasks = user.Tasks.Select(task => new ShortTodoItemDto
                {
                    Id = (int)task.Id,
                    Name = task.Name,
                    IsCompleted = task.IsComplete
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}