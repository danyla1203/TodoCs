using todo.Models;
using Microsoft.EntityFrameworkCore;

namespace todo.Data.Repositories;

public class TodoRepository : BaseRepository<TodoItem>
{
    public TodoRepository(DbSet<TodoItem> context)
        : base(context)
    {}


}
