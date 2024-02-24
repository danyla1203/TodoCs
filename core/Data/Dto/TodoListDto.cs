using todo.Models;

namespace todo.Data.Dto;

public class TodoListDto
{
  public List<TodoItem> items { get; set; } = new List<TodoItem>();
  public int count { get; set; }
}