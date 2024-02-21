using AutoMapper;
using Microsoft.EntityFrameworkCore;
using todo.Data;
using todo.Data.Dto;
using todo.Data.Repositories;
using todo.Models;
using todo.Services;

var builder = WebApplication.CreateBuilder(args);

var config = new MapperConfiguration(cfg => cfg.CreateMap<TodoItem, TodoItemDto>());
var mapper = config.CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ApplicationContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }