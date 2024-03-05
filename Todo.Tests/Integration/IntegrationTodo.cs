using AutoFixture;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using todo.Data.Dto;
using System.Text.Json;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using todo.Models;
using todo.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace todo.Tests.Integration;

public class TodoIntegration : IClassFixture<WebAppFactory<Program>>, IDisposable
{
  private Fixture fixture = new Fixture();
  private readonly WebAppFactory<Program> _factory;
  private readonly HttpClient _client;
  private readonly ApplicationDbContext _context;

  public TodoIntegration(WebAppFactory<Program> factory)
  {
    _factory = factory;
    var scope = factory.Services.CreateScope();
    _context = scope.ServiceProvider.GetService<ApplicationDbContext>();
    _context.Database.EnsureCreated();
    _client = _factory.CreateClient();
  }

  private async Task<User> AddUser()
  {
    var user = new User
    {
      FirstName = fixture.Create<string>(),
      LastName = fixture.Create<string>(),
      Email = fixture.Create<string>(),
      Password = fixture.Create<string>(),
    };
    await _context.Users.AddAsync(user);
    return user;
  }

  public class ItemAndDto
  {
    public TodoItemDto Dto;
    public TodoItem Model;
  }
  private async Task<ItemAndDto> AddTodo(
    int? Id = null,
#nullable enable
    string? Name = null,
    bool? IsCompleted = null)
  {
    var data = new TodoItem
    {
      Id = Id ?? fixture.Create<long>(),
      Name = Name ?? fixture.Create<string>(),
      IsCompleted = IsCompleted ?? false
    };
    await _context.AddAsync(data);
    return new ItemAndDto
    {
      Dto = new TodoItemDto
      {
        Id = (int)data.Id,
        Name = data.Name,
        IsCompleted = data.IsCompleted
      },
      Model = data
    };
  }

  public void Dispose()
  {
    _context.TodoItems.ExecuteDelete();
  }

  [Fact]
  public async Task GetTodoList_WithoutParams()
  {
    //Arrange
    var item = (await AddTodo(Name: "TestTodo")).Dto;
    await _context.SaveChangesAsync();
    var expected = new TodoListDto
    {
      count = 1,
      items = new List<TodoItemDto> { item }
    };
    //Act
    var response = await _client.GetAsync("/todo");
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoListDto>(body, options);
    result.Should().BeEquivalentTo(expected);
  }
  [Fact]
  public async Task GetTodoList_WithIncorrectParams()
  {
    //Act
    var response = await _client.GetAsync("/todo?completed=7");
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
  }
  [Fact]
  public async Task GetTodoList_WithFiltering_OnlyCompletedTodo()
  {
    //Arrange
    await AddTodo();
    await AddTodo();
    var item = (await AddTodo(IsCompleted: true)).Dto;
    await _context.SaveChangesAsync();
    TodoListDto expected = new TodoListDto
    {
      count = 1,
      items = new List<TodoItemDto> { item }
    };
    //Act
    var response = await _client.GetAsync("/todo?completed=true");
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoListDto>(body, options);
    result.Should().BeEquivalentTo(expected);
  }
  [Fact]
  public async Task GetTodoList_WithFiltering_OnlyIncompletedTodo()
  {
    //Arrange
    var todo = (await AddTodo()).Dto;
    var todo2 = (await AddTodo()).Dto;
    await AddTodo(IsCompleted: true);
    await _context.SaveChangesAsync();
    TodoListDto expected = new TodoListDto
    {
      count = 2,
      items = new List<TodoItemDto> { todo, todo2 }
    };
    //Act
    var response = await _client.GetAsync("/todo?completed=false");
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoListDto>(body, options);
    result.Should().BeEquivalentTo(expected);
  }
  [Fact]
  public async Task GetTodoItem_Successfully()
  {
    //Arrange
    var todo = (await AddTodo()).Model;
    await _context.SaveChangesAsync();
    TodoItem expected = todo;
    //Act
    var response = await _client.GetAsync($"/todo/{todo.Id}");
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoItem>(body, options);
    result.Should().BeEquivalentTo(expected);
  }
  [Fact]
  public async Task GetTodoItem_404Exception()
  {
    //Act
    var response = await _client.GetAsync("/todo/666");
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
  }
  [Fact]
  public async Task GetTodoItem_WithIncorrectId()
  {
    //Act
    var response = await _client.GetAsync("/todo/null");
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
  }
  [Fact]
  public async Task AddTodoItem_Successfully()
  {
    //Arrange
    var inputTodo = new TodoItemDto { Name = "New todo" };
    //Act
    var response = await _client.PostAsJsonAsync("/Todo", inputTodo);
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoItem>(body, options);
    result?.Name.Should().BeEquivalentTo("New todo");
    result?.IsCompleted.Should().BeFalse();
  }
  [Fact]
  public async Task AddTodoItem_IncorrectBody()
  {
    //Arrange
    var inputTodo = new { };
    //Act
    var response = await _client.PostAsJsonAsync("/Todo", inputTodo);
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
  }
  [Fact]
  public async Task DeleteTodoItem_Successfully()
  {
    //Arrange
    var todoInDb = (await AddTodo()).Model;
    await _context.SaveChangesAsync();
    //Act
    var response = await _client.DeleteAsync($"/Todo/{todoInDb.Id}");
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoItem>(body, options);
    result.Should().BeEquivalentTo(todoInDb);
  }
  [Fact]
  public async Task DeleteTodoItem_404Exception()
  {
    //Act
    var response = await _client.DeleteAsync("/Todo/666");
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
  }
  [Fact]
  public async Task DeleteTodoItem_WithIncorrectParam()
  {
    //Act
    var response = await _client.DeleteAsync("/Todo/null");
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
  }
  [Fact]
  public async Task AssignTaskToUser_Successfully()
  {
    //Arrange
    var todoInDb = (await AddTodo()).Dto;
    var user = await AddUser();
    TodoItemDto expected = new TodoItemDto
    {
      Id = todoInDb.Id,
      Name = todoInDb.Name,
      IsCompleted = todoInDb.IsCompleted,
      Performer = new TaskPerformer
      {
        Id = 1,
        FirstName = user.FirstName,
        LastName = user.LastName,
        DisplayName = user.LastName + " " + user.FirstName,
        Email = user.Email
      }
    };
    await _context.SaveChangesAsync();
    //Act
    var response = await _client.PatchAsync($"/Todo/{todoInDb.Id}?userId={user.Id}", null);
    //Assert
    response.EnsureSuccessStatusCode();
    var body = await response.Content.ReadAsStreamAsync();
    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    var result = await JsonSerializer.DeserializeAsync<TodoItemDto>(body, options);
    result.Should().BeEquivalentTo(expected);
  }
  [Fact]
  public async Task AssignTaskToUser_UserAlreadyAssigned()
  {
    //Arrange
    var todoInDb = (await AddTodo()).Model;
    var user = await AddUser();
    user.Tasks.Add(todoInDb);
    await _context.SaveChangesAsync();
    //Act
    var response = await _client.PatchAsync($"/Todo/{todoInDb.Id}?userId={user.Id}", null);
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
  }
  [Fact]
  public async Task AssignTaskToUser_TodoItemNotFound()
  {
    //Arrange
    var user = await AddUser();
    await _context.SaveChangesAsync();
    //Act
    var response = await _client.PatchAsync($"/Todo/666?userId={user.Id}", null);
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
  }
  [Fact]
  public async Task AssignTaskToUser_UserItemNotFound()
  {
    //Arrange
    var todoInDb = (await AddTodo()).Model;
    await _context.SaveChangesAsync();
    //Act
    var response = await _client.PatchAsync($"/Todo/{todoInDb.Id}?userId=666", null);
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
  }
}