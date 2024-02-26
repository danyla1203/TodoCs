using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using todo.Data.Dto;
using todo.Data.Repositories;
using todo.Exceptions;
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
        var stub = fixture.Create<User>();
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
            Email = stub.Email
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
        var createdUser = fixture.Create<User>();
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
        var createdUser = fixture.Create<User>();
        var expected = new CreatedUserDto
        {
            Id = createdUser.Id,
            FirstName = createdUser.FirstName,
            LastName = createdUser.LastName,
            Email = createdUser.Email
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
}