using todo.Data.Dto;
using todo.Exceptions;
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

    public async Task<List<TodoItemDto>> GetTodoList(bool? completed = null)
    {
        return await _unit.TodoRepository.GetTodoItemsList(completed);
    }
    public async Task<TodoItemDto> GetTodoItem(int id)
    {
        TodoItemDto? item = await _unit.TodoRepository.GetTodoItem(id);
        return item ?? throw new TodoItemNotFound();
    }
    public async Task<TodoItem> AddTodoItem(AddTodoItemDto todoItem)
    {
        var item = new TodoItem
        {
            Name = todoItem.Name,
            IsCompleted = todoItem.IsComplete ?? false
        };
        var newRecord = await _unit.TodoRepository.AddItem(item);
        await _unit.Save();
        return newRecord;
    }
    public async Task<TodoItemDto> DeleteTodoItem(int id)
    {
        TodoItemDto? record = await _unit.TodoRepository.GetTodoItem(id);
        if (record == null) throw new TodoItemNotFound();

        await _unit.TodoRepository.Delete(id);
        await _unit.Save();
        return record;
    }
    public async Task AssignTaskToUser(int taskId, int userId)
    {
        TodoItem? task = await _unit.TodoRepository.GetById(taskId);
        if (task == null) throw new TodoItemNotFound();
        User? user = await _unit.UserRepository.GetById(userId);
        if (user == null) throw new UserNotFound();

        user.Tasks.Add(task);
        await _unit.Save();
    }
}