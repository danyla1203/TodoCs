namespace todo.Data.Dto;

public class TodoListDto
{
  public List<TodoItemDto> items { get; set; }
  public int count { get; set; }
}