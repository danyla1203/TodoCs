using todo.Data.Dto;
using todo.Models;

namespace todo.Services;

public interface ITodoService
{
  List<TodoItem> GetTodoList(bool? completed);
  TodoItem? GetTodoItem(int id);
  TodoItem AddTodoItem(TodoItem todoItem);
  TodoItem? DeleteTodoItem(int id);
}