using System.ComponentModel.DataAnnotations;

namespace todo.Models;

public class TodoItem
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}