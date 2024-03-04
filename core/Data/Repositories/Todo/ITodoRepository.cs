using todo.Data.Dto;
using todo.Models;

namespace todo.Data.Repositories;

public interface ITodoRepository : IRepositoryBase<TodoItem>
{
  Task<TodoItemDto?> GetTodoItem (int id);
  Task<List<TodoItemDto>> GetTodoItemsList (bool? completedStatus);
}