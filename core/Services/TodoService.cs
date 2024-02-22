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

    public async Task<TodoItem?> GetTodoItem(int id)
    {
        TodoItem? item = await _unit.TodoRepository.GetById(id);
        return item ?? throw new TodoItemNotFound(); 
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
        if (record == null) throw new TodoItemNotFound();

        await _unit.TodoRepository.Delete(id);
        await _unit.Save();
        return record;
    } 
}