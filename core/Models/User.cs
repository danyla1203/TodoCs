using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace todo.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    public required string FirstName { get; set; }
    [Required]
    [StringLength(30)]
    public required string LastName { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 6)]
    public required string Password { get; set; }

    public string DisplayName { get; private set; }

    public ICollection<TodoItem> tasks { get; } = new List<TodoItem>();
}