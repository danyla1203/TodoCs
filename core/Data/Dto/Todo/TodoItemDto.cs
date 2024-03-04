namespace todo.Data.Dto;

public class TodoItemDto
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public bool IsCompleted { get; set; }
  public TaskPerformer? Performer { get; set; }
}