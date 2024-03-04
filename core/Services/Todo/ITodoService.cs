using todo.Data.Dto;
using todo.Models;

namespace todo.Services;

public interface ITodoService
{
    Task<List<TodoItemDto>> GetTodoList(bool? completed);
    Task<TodoItemDto> GetTodoItem(int id);
    Task<TodoItem> AddTodoItem(AddTodoItemDto todoItem);
    Task<TodoItemDto> DeleteTodoItem(int id);
    Task AssignTaskToUser(int taskId, int userId);
}