using Xunit;
using Moq;
using System;
using todo.Services;
using todo.Data.Repositories;
using System.Collections.Generic;
using todo.Models;
using AutoFixture;
using System.Linq.Expressions;
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
        var stub = new List<TodoItem>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo
            .Setup(rep => rep.GetAll(It.IsAny<Expression<Func<TodoItem, bool>>>()))
            .Returns(Task.FromResult(new List<TodoItem>()));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        var result = await service.GetTodoList();
        //Assert
        result.Should().BeEquivalentTo(stub);
    }

    [Fact]
    public async Task GetTodoList_OnCompletedTodo()
    {
        //Arrange
        var dbStub = new List<TodoItem> {
            fixture.Build<TodoItem>().With(t => t.IsComplete, true).Create(),
        };
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo
            .Setup(rep => rep.GetAll(It.IsAny<Expression<Func<TodoItem, bool>>>()))
            .Returns(Task.FromResult(dbStub));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        var expected = new List<TodoItem> {
            new TodoItem {
                Id = dbStub[0].Id,
                IsComplete = dbStub[0].IsComplete,
                Name = dbStub[0].Name
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
        var stub = fixture.Create<TodoItem>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
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
        var inputTodo = fixture.Create<TodoItemDto>();
        var expected = fixture.Create<TodoItem>();
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
        var item = fixture.Create<TodoItem>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(item));
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act
        var result = await service.DeleteTodoItem((int)item.Id);
        //Assert
        result.Should().BeEquivalentTo(item);
        mockUnit.Verify(unit => unit.Save(), Times.Once());
        mockRepo.Verify(rep => rep.Delete((int)item.Id), Times.Once());
    }
    [Fact]
    public async Task DeleteTodoItemNotFound_Unit()
    {
        //Arrange
        var mockRepo = new Mock<ITodoRepository>();
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);
        //Act, Assert
        await Assert.ThrowsAsync<TodoItemNotFound>(() => service.DeleteTodoItem(-1));
    }
}