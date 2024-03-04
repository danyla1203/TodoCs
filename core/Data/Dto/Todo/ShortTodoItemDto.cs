namespace todo.Data.Dto;

public class ShortTodoItemDto
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public bool IsCompleted { get; set; }
}