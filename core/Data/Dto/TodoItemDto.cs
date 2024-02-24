namespace todo.Data.Dto;

public class TodoItemDto
{
  public required string Name { get; set; }
  public bool? IsComplete { get; set; }
}