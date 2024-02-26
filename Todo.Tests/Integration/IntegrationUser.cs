using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using todo.Data;
using todo.Data.Dto;
using todo.Models;
using Xunit;

namespace todo.Tests.Integration;

public class UserIntegration : IClassFixture<WebAppFactory<Program>>, IDisposable
{
    private Fixture fixture = new Fixture();
    private readonly WebAppFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;
    public UserIntegration(WebAppFactory<Program> factory)
    {
        _factory = factory;
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        _context.Database.EnsureCreated();
        _client = _factory.CreateClient();
    }
    public void Dispose()
    {
        _context.Users.ExecuteDelete();
    }

    public class InputOutputUser
    {
        public AddUserDto Input;
        public CreatedUserDto Created;
    }

    public async Task<InputOutputUser> CreateUser(User user)
    {
        await _context.AddAsync(user);
        var addUserDto = new AddUserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Password = user.Password
        };
        var createdUserDto = new CreatedUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
        return new InputOutputUser 
        { 
            Input = addUserDto, 
            Created = createdUserDto 
        };
    }

    [Fact]
    public async Task AddUser_Successfully()
    {
        //Arrange
        var inputUser = fixture.Create<AddUserDto>();
        //Act
        var response = await _client.PostAsJsonAsync("/User", inputUser);
        //Assert
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = await JsonSerializer.DeserializeAsync<CreatedUserDto>(body, options);
        result.FirstName.Should().BeEquivalentTo(inputUser.FirstName);
        result.LastName.Should().BeEquivalentTo(inputUser.LastName);
        result.Email.Should().BeEquivalentTo(inputUser.Email);
    }
    [Fact]
    public async Task AddUser_ConflictError()
    {
        //Arrange
        var userInDb = fixture.Create<User>();
        var input = (await CreateUser(userInDb)).Input;
        await _context.SaveChangesAsync();
        //Act
        var response = await _client.PostAsJsonAsync("/User", input);
        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetUser_Successfully()
    {
        //Arrange
        var userInDb = fixture.Create<User>();
        var created = (await CreateUser(userInDb)).Created;
        await _context.SaveChangesAsync();
        //Act
        var response = await _client.GetAsync($"/User/{created.Id}");
        //Assert
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStreamAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = await JsonSerializer.DeserializeAsync<CreatedUserDto>(body, options);
        result.Should().BeEquivalentTo(created);
    }
    [Fact]
    public async Task GetUser_NotFound()
    {
        //Act
        var response = await _client.GetAsync("/User/666");
        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}