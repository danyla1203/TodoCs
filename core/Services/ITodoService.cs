using todo.Data.Dto;
using todo.Models;

namespace todo.Services;

public interface ITodoService
{
  TodoListDto GetTodoList(bool? completed);
  TodoItem? GetTodoItem(int id);
  TodoItem AddTodoItem(TodoItem todoItem);
  TodoItem? DeleteTodoItem(int id);
}