using todo.Data.Dto;
using todo.Models;

namespace todo.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetTodoList(bool? completed);
    Task<TodoItem> GetTodoItem(int id);
    Task<TodoItem> AddTodoItem(TodoItemDto todoItem);
    Task<TodoItem> DeleteTodoItem(int id);
}