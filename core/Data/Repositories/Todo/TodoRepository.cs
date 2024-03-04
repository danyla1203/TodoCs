using Microsoft.EntityFrameworkCore;
using todo.Data.Dto;
using todo.Models;

namespace todo.Data.Repositories;

public class TodoRepository : BaseRepository<TodoItem>, ITodoRepository
{
    public TodoRepository(ApplicationDbContext context)
        : base(context)
    { }

    public async Task<List<TodoItemDto>> GetTodoItemsList(bool? completedStatus)
    {
        IQueryable<TodoItem> query = _table.Include(task => task.Performer);
        if (completedStatus != null)
        {
            query = query.Where(todo => todo.IsCompleted == completedStatus);
        }
        return await query.Select(task => new TodoItemDto
        {
            Id = (int)task.Id,
            Name = task.Name,
            IsCompleted = task.IsCompleted,
            Performer = task.Performer != null ? new TaskPerformer
            {
                Id = task.Performer.Id,
                LastName = task.Performer.LastName,
                FirstName = task.Performer.FirstName,
                Email = task.Performer.Email,
                DisplayName = task.Performer.DisplayName
            } : null
        }).ToListAsync();
    }

    public Task<TodoItemDto?> GetTodoItem(int id)
    {
        return _table
            .Include(task => task.Performer)
            .Where(task => task.Id == id)
            .Select(task => new TodoItemDto
            {
                Id = (int)task.Id,
                Name = task.Name,
                IsCompleted = task.IsCompleted,
                Performer = task.Performer != null ? new TaskPerformer
                {
                    Id = task.Performer.Id,
                    LastName = task.Performer.LastName,
                    DisplayName = task.Performer.DisplayName,
                    FirstName = task.Performer.FirstName,
                    Email = task.Performer.Email
                } : null
            })
            .FirstOrDefaultAsync();
    }

}
