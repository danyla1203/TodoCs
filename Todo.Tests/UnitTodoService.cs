using Xunit;
using Moq;
using todo.Services;
using todo.Data.Repositories;
using System.Collections.Generic;
using todo.Models;
using AutoFixture;
using Todo.Tests.Utils.Mock;
namespace Todo.Tests;

public class TodoServiceUnitTest
{
    private Fixture fixture = new Fixture();

    [Fact]
    public void GetTodoList_Unit()
    {
        var stub = new List<TodoItem>();
        var mockUnit = new Mock<MockUnitOfWork>();
        TodoService service = new TodoService(mockUnit.Object);

        var result = service.GetTodoList();
        Assert.Equal(stub, result);
    }

    [Fact]
    public void GetTodoItemById_Unit()
    {
        var stub = fixture.Create<TodoItem>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(stub);
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);

        TodoService service = new TodoService(mockUnit.Object);

        var result = service.GetTodoItem(fixture.Create<int>());
        Assert.Equal(stub, result);
    }

    [Fact]
    public void AddTodoItem_Unit()
    {
        var item = fixture.Create<TodoItem>();
        var mockRepo = new Mock<ITodoRepository>();
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);

        var result = service.AddTodoItem(item);
        Assert.Equal(item, result);
        mockRepo.Verify(rep => rep.AddItem(item), Times.Once());
        mockUnit.Verify(unit => unit.Save(), Times.Once());
    }

    [Fact]
    public void DeleteTodoItem_Unit()
    {
        var item = fixture.Create<TodoItem>();
        var mockRepo = new Mock<ITodoRepository>();
        mockRepo.Setup(rep => rep.GetById(It.IsAny<int>()))
            .Returns(item);
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);

        var result = service.DeleteTodoItem((int)item.Id);
        Assert.Equal(item, result);
        mockUnit.Verify(unit => unit.Save(), Times.Once());
        mockRepo.Verify(rep => rep.Delete((int)item.Id), Times.Once());
    }
    [Fact]
    public void DeleteTodoItemNotFound_Unit()
    {
        var mockRepo = new Mock<ITodoRepository>();
        var mockUnit = new Mock<MockUnitOfWork>(mockRepo.Object);
        TodoService service = new TodoService(mockUnit.Object);

        var result = service.DeleteTodoItem(-1);
        Assert.Null(result);
        mockUnit.Verify(unit => unit.Save(), Times.Never());
        mockRepo.Verify(rep => rep.Delete(-1), Times.Never());
    }
}