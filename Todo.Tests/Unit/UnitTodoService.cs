using Xunit;
using Moq;
using todo.Services;
using todo.Data.Repositories;
using System.Collections.Generic;
using todo.Models;
using AutoFixture;
using Todo.Tests.Utils.Mock;
using todo.Data.Dto;
using FluentAssertions;
using System.Threading.Tasks;
using todo.Exceptions.TodoExceptions;

namespace Todo.Tests;

public class TodoServiceUnitTest
{
    private Fixture fixture = new Fixture();

    [Fact]
    public async Task GetTodoList_WithoutParams()
    {
        // Arrange
        var stub = new List<TodoItemDto>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo
            .Setup(rep => rep.GetTodoItemsList(It.IsAny<bool>()))
            .Returns(Task.FromResult(stub));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        var result = await service.GetTodoList(false);
        //Assert
        result.Should().BeEquivalentTo(stub);
    }

    [Fact]
    public async Task GetTodoList_OnCompletedTodo()
    {
        //Arrange
        var dbStub = new List<TodoItemDto> {
            new TodoItemDto 
            {
                Id = fixture.Create<int>(),
                Name = fixture.Create<string>(),
                IsCompleted = true,
                Performer = fixture.Create<TaskPerformer>()
            }
        };
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo
            .Setup(rep => rep.GetTodoItemsList(It.IsAny<bool>()))
            .Returns(Task.FromResult(dbStub));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        var expected = new List<TodoItemDto> {
            new TodoItemDto {
                Id = dbStub[0].Id,
                IsCompleted = dbStub[0].IsCompleted,
                Name = dbStub[0].Name,
                Performer = dbStub[0].Performer
            }
        };
        //Act
        var result = await service.GetTodoList(true);
        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetTodoItemById_Unit()
    {
        //Arrange
        var stub = new TodoItemDto
        {
            Id = fixture.Create<int>(),
            Name = fixture.Create<string>(),
            IsCompleted = fixture.Create<bool>(),
            Performer = null
        };
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.GetTodoItem(It.IsAny<int>()))
            .Returns(Task.FromResult(stub));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        var result = await service.GetTodoItem(fixture.Create<int>());
        //Assert
        result.Should().BeEquivalentTo(stub);
    }
    [Fact]
    public async Task GetTodoItemById_NotFound()
    {
        //Arrange
        var mockRepo = new Mock<ITodoRepository>();
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act, Assert
        await Assert.ThrowsAsync<TodoItemNotFound>(() => service.GetTodoItem(-1));
    }

    [Fact]
    public async Task AddTodoItem_Unit()
    {
        //Arrange
        var inputTodo = fixture.Create<AddTodoItemDto>();
        var expected = new TodoItem
        {
            Name = fixture.Create<string>(),
            IsCompleted = fixture.Create<bool>(),
        };
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.AddItem(It.IsAny<TodoItem>()))
            .Returns(Task.FromResult(expected));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        var result = await service.AddTodoItem(inputTodo);
        //Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task DeleteTodoItem_Unit()
    {
        //Arrange
        var item = new TodoItemDto
        {
            Id = fixture.Create<int>(),
            Name = fixture.Create<string>(),
            IsCompleted = fixture.Create<bool>(),
            Performer = null
        };
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.GetTodoItem(It.IsAny<int>()))
            .Returns(Task.FromResult(item));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        var result = await service.DeleteTodoItem(item.Id);
        //Assert
        result.Should().BeEquivalentTo(item);
        mockUnit.Verify(unit => unit.Save(), Times.Once());
        mockRepo.Verify(rep => rep.Delete(item.Id), Times.Once());
    }
    [Fact]
    public async Task DeleteTodoItemNotFound_Unit()
    {
        //Arrange
        var mockRepo = new Mock<ITodoRepository>();
        var mockUnit = new Mock<MockUnitOfWork>();
        TodoService service = new TodoService(mockUnit.Object);
        //Act, Assert
        await Assert.ThrowsAsync<TodoItemNotFound>(() => service.DeleteTodoItem(-1));
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
            Id = 1,
            Name = "Test todo",
            IsCompleted = false,
        };
        var expected = new CreatedUserDto
        {
            Id = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            DisplayName = createdUser.DisplayName,
            Email = createdUser.Email,
            Tasks = new List<ShortTodoItemDto> { new ShortTodoItemDto {
                Id = (int)createdTodo.Id,
                Name = createdTodo.Name,
                IsCompleted = createdTodo.IsCompleted
            } }
        };
        var mockUserRepo = new Mock<IUserRepository>();
        mockUserRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(createdUser));
        var mockTodoRepo = new Mock<ITodoRepository>();
        mockTodoRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(createdTodo));
        var mockUnit = new Mock<MockUnitOfWork>(mockTodoRepo.Object, mockUserRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        await service.AssignTaskToUser((int)createdTodo.Id, (int)createdUser.Id);
        //Assert

    }
    [Fact]
    public async Task AssignTaskToUser__UserNotFound()
    {
        //Arrange
        var mockUnit = new Mock<MockUnitOfWork>();
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        await Assert.ThrowsAsync<TodoItemNotFound>(() => service.AssignTaskToUser(1, -1));
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
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        await Assert.ThrowsAsync<TodoItemNotFound>(() => service.AssignTaskToUser(-1, 1));
    }
}