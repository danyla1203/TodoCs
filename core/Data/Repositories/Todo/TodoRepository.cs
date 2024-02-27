using todo.Models;

namespace todo.Data.Repositories;

public class TodoRepository : BaseRepository<TodoItem>, ITodoRepository
{
    public TodoRepository(ApplicationDbContext context)
        : base(context)
    {}
}
