using todo.Models;

namespace todo.Data.Dto;

public class TodoListDto
{
  public List<TodoItemDto> items { get; set; } = new List<TodoItemDto>();
  public int count { get; set; }
}