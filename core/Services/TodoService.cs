using todo.Data.Dto;
using todo.Exceptions.TodoExceptions;
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
    public async Task<TodoItem> GetTodoItem(int id)
    {
        TodoItem? item = await _unit.TodoRepository.GetById(id);
        return item ?? throw new TodoItemNotFound();
    }
    public async Task<TodoItem> AddTodoItem(TodoItemDto todoItem)
    {
        var item = new TodoItem
        {
            Name = todoItem.Name,
            IsComplete = todoItem.IsComplete ?? false
        };
        var newRecord = await _unit.TodoRepository.AddItem(item);
        await _unit.Save();
        return newRecord;
    }
    public async Task<TodoItem> DeleteTodoItem(int id)
    {
        TodoItem? record = await _unit.TodoRepository.GetById(id);
        if (record == null) throw new TodoItemNotFound();

        await _unit.TodoRepository.Delete(id);
        await _unit.Save();
        return record;
    }
}