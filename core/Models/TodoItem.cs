﻿using System.ComponentModel.DataAnnotations;

namespace todo.Models;

public class TodoItem
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    public required bool IsComplete { get; set; }
}