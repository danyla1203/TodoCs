using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using todo.Data.Dto;
using todo.Data.Repositories;
using todo.Exceptions;
using todo.Exceptions.TodoExceptions;
using todo.Models;
using todo.Services;
using Todo.Tests.Utils.Mock;
using Xunit;

namespace Todo.Tests;

public class UnitUserService
{
    private Fixture fixture = new Fixture();

    [Fact]
    public async Task CreateUser_WithCorrectParams()
    {
        //Arrange
        var stub = new User
        {
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = fixture.Create<string>(),
            Password = fixture.Create<string>(),
        };
        var userDto = new AddUserDto
        {
            FirstName = stub.FirstName,
            LastName = stub.LastName,
            Email = stub.Email,
            Password = stub.Password
        };
        var result = new CreatedUserDto
        {
            Id = stub.Id,
            FirstName = stub.FirstName,
            LastName = stub.LastName,
            Email = stub.Email,
            Tasks = new List<TodoItem>()
        };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(rep => rep.AddItem(It.IsAny<User>()))
            .Returns(Task.FromResult(stub));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        UserService service = new UserService(mockUnit.Object);
        //Act
        var newUser = await service.CreateUser(userDto);
        //Assert
        newUser.Should().BeEquivalentTo(result);
    }
    [Fact]
    public async Task CreateUser_ConflictError()
    {
        //Arrange
        var createdUser = new User
        {
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = fixture.Create<string>(),
            Password = fixture.Create<string>(),
        };
        var input = new AddUserDto
        {
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Password = fixture.Create<string>(),
            Email = createdUser.Email
        };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(rep => rep.FindUserByEmail(It.IsAny<string>()))
            .Returns(Task.FromResult(createdUser));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        UserService service = new UserService(mockUnit.Object);
        //Act, Assert
        await Assert.ThrowsAsync<UserAlreadyExist>(() => service.CreateUser(input));
    }
    [Fact]
    public async Task GetUser_Successfully()
    {
        //Arrange
        var createdUser = new User
        {
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = fixture.Create<string>(),
            Password = fixture.Create<string>(),
        };
        var expected = new CreatedUserDto
        {
            Id = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            Email = createdUser.Email,
            Tasks = new List<TodoItem>()
        };
        var mockRepo = new Mock<IUserRepository>();
        mockRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(createdUser));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        UserService service = new UserService(mockUnit.Object);
        //Act
        var result = await service.GetUser((int)createdUser.Id);
        //Assert
        result.Should().BeEquivalentTo(expected);
    }
    [Fact]
    public async Task GetUser_NotFound()
    {
        var mockRepo = new Mock<IUserRepository>();
        var mockUnit = new Mock<MockUnitOfWork>();
        UserService service = new UserService(mockUnit.Object);
        //Act, Assert
        await Assert.ThrowsAsync<UserNotFound>(() => service.GetUser(-1));
    }
    [Fact]
    public async Task AssingTaskToUser_Successfully()
    {
        //Arrange
        var createdUser = new User
        {
            Id = (long)1,
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = fixture.Create<string>(),
            Password = fixture.Create<string>(),
        };
        var createdTodo = new TodoItem
        {
            Id = (long)1,
            Name = "Test todo",
            IsComplete = false,
        };
        var expected = new CreatedUserDto
        {
            Id = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            Email = createdUser.Email,
            Tasks = new List<TodoItem> { createdTodo }
        };
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(createdUser));
        var mockTodoRepo = new Mock<ITodoRepository>();
        mockTodoRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(createdTodo));
        var mockUnit = new Mock<MockUnitOfWork>(mockTodoRepo.Object, mockUserRepo.Object);
        UserService service = new UserService(mockUnit.Object);
        //Act
        var result = await service.AssignTaskToUser((int)createdTodo.Id, (int)createdUser.Id);
        //Assert
        result.Should().BeEquivalentTo(expected);
    }
    [Fact]
    public async Task AssignTaskToUser__UserNotFound()
    {
        //Arrange
        var mockUnit = new Mock<MockUnitOfWork>();
        UserService service = new UserService(mockUnit.Object);
        //Act
        await Assert.ThrowsAsync<UserNotFound>(() => service.AssignTaskToUser(1, -1));
    }
    [Fact]
    public async Task AssignTaskToUser_TodoItemNotFound()
    {
        //Arrange
        var mockUserInDb = new User
        {
            Id = (long)1,
            FirstName = fixture.Create<string>(),
            LastName = fixture.Create<string>(),
            Email = fixture.Create<string>(),
            Password = fixture.Create<string>(),
        };
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(mockUserInDb));
        var mockUnit = new Mock<MockUnitOfWork>(mockUserRepo.Object);
        UserService service = new UserService(mockUnit.Object);
        //Act
        await Assert.ThrowsAsync<TodoItemNotFound>(() => service.AssignTaskToUser(-1, 1));
    }
}