namespace todo.Data.Dto;

public class TaskPerformer
{
  public required long Id { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string DisplayName { get; set; }
  public required string Email { get; set; }
}