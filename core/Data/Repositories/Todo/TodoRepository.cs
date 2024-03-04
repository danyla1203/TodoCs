using Microsoft.EntityFrameworkCore;
using todo.Data.Dto;
using todo.Models;

namespace todo.Data.Repositories;

public class TodoRepository : BaseRepository<TodoItem>, ITodoRepository
{
    public TodoRepository(ApplicationDbContext context)
        : base(context)
    { }

    public Task<List<TodoItemDto>> GetTodoItemsList(bool? completedStatus)
    {
        var query = _table.Include(task => task.performer);
        if (completedStatus != null) 
        {
            query.Where(task => task.IsComplete == completedStatus);
        }
        return query.Select(task => new TodoItemDto
        {
            Id = (int)task.Id,
            Name = task.Name,
            IsCompleted = task.IsComplete,
            Performer = task.performer != null ? new TaskPerformer
            {
                Id = task.performer.Id,
                LastName = task.performer.LastName,
                FirstName = task.performer.FirstName,
                Email = task.performer.Email,
                DisplayName = task.performer.DisplayName
            } : null
        }).ToListAsync();
    }

    public Task<TodoItemDto?> GetTodoItem(int id)
    {
        return _table
            .Include(task => task.performer)
            .Where(task => task.Id == id)
            .Select(task => new TodoItemDto
            {
                Id = (int)task.Id,
                Name = task.Name,
                IsCompleted = task.IsComplete,
                Performer = task.performer != null ? new TaskPerformer
                {
                    Id = task.performer.Id,
                    LastName = task.performer.LastName,
                    DisplayName = task.performer.DisplayName,
                    FirstName = task.performer.FirstName,
                    Email = task.performer.Email
                } : null
            })
            .FirstOrDefaultAsync();
    }

}
