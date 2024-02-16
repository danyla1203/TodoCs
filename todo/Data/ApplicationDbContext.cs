﻿using Microsoft.EntityFrameworkCore;
using todo.Models;
namespace todo.Data;

public class ApplicationDbContext : DbContext
{
    protected readonly IConfiguration _configuration;

    public ApplicationDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionstring = _configuration.GetConnectionString("ApplicationContext");
        optionsBuilder.UseNpgsql(connectionstring);
    }


    public DbSet<TodoItem> TodoItems { get; set; }
}