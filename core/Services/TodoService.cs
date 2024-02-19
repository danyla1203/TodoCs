using todo.Data.Dto;
using todo.Models;

namespace todo.Services;

public class TodoService : ITodoService
{
    private readonly IUnitOfWork _unit;

    public TodoService(IUnitOfWork unit)
    {
        _unit = unit;
    }

    public async Task<List<TodoItem>> GetTodoList(bool? completed = null)
    {
        return await _unit.TodoRepository.GetAll(
            completed != null ?
            todo => todo.IsComplete == completed : null
        );
    }

    public Task<TodoItem?> GetTodoItem(int id)
    {
        return _unit.TodoRepository.GetById(id);
    }
    public async Task<TodoItem> AddTodoItem(TodoItem todoItem)
    {
        await _unit.TodoRepository.AddItem(todoItem);
        await _unit.Save();
        return todoItem;
    }
    public async Task<TodoItem?> DeleteTodoItem(int id)
    {
        TodoItem? record = await _unit.TodoRepository.GetById(id);
        if (record != null)
        {
            await _unit.TodoRepository.Delete(id);
            await _unit.Save();
        }
        return record;
    }
}