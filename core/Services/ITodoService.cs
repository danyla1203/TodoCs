using todo.Models;

namespace todo.Services;

public interface ITodoService
{
  IEnumerable<TodoItem> GetTodoList();
  TodoItem? GetTodoItem(int id);
  TodoItem AddTodoItem(TodoItem todoItem);
  TodoItem? DeleteTodoItem(int id);
}