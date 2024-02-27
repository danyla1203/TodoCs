using todo.Models;

namespace todo.Data.Dto;

public class CreatedUserDto
{
    public required long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public required string Email { get; set; }

    public ICollection<TodoItem> Tasks { get; set; }
}