namespace todo.Data.Dto;

public class AddTodoItemDto
{
  public required string Name { get; set; }
  public bool? IsComplete { get; set; }
}