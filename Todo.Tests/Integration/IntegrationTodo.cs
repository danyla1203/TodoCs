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

  private async Task<TodoItem> AddTodo(
    int? Id = null,
#nullable enable
    string? Name = null,
    bool? IsComplete = null)
  {
    var data = new TodoItem
    {
      Id = Id ?? fixture.Create<long>(),
      Name = Name ?? fixture.Create<string>(),
      IsComplete = IsComplete ?? false
    };
    await _context.AddAsync(data);
    return new TodoItem
    {
      Id = data.Id,
      Name = data.Name,
      IsComplete = data.IsComplete
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
    var item = await AddTodo(Name: "TestTodo");
    await _context.SaveChangesAsync();
    var expected = new TodoListDto
    {
      count = 1,
      items = new List<TodoItem> {
        new TodoItem { Id = item.Id, Name = item.Name, IsComplete = false }
      }
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
  public async Task GetTodoList_WithFiltering_OnlyCompletedTodo()
  {
    //Arrange
    await AddTodo();
    await AddTodo();
    var item = await AddTodo(IsComplete: true);
    await _context.SaveChangesAsync();
    TodoListDto expected = new TodoListDto
    {
      count = 1,
      items = new List<TodoItem> { item }
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
    var todo = await AddTodo();
    var todo2 = await AddTodo();
    await AddTodo(IsComplete: true);
    await _context.SaveChangesAsync();
    TodoListDto expected = new TodoListDto
    {
      count = 2,
      items = new List<TodoItem> { todo, todo2 }
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
    var todo = await AddTodo();
    await _context.SaveChangesAsync();
    TodoItem expected = new TodoItem
    {
      Id = todo.Id,
      Name = todo.Name,
      IsComplete = todo.IsComplete
    };
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
    result?.IsComplete.Should().BeFalse();
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
    var todoInDb = await AddTodo();
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
    //Arrange
    await _context.SaveChangesAsync();
    //Act
    var response = await _client.DeleteAsync("/Todo/666");
    //Assert
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
  }

}