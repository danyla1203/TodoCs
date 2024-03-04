using System.Threading.Tasks;
using Moq;
using todo.Data.Repositories;

namespace Todo.Tests.Utils.Mock;

public class MockUnitOfWork : IUnitOfWork
{
    ITodoRepository todoRepository;
    IUserRepository userRepository;
    public MockUnitOfWork(ITodoRepository mock)
    {
      todoRepository = mock;
    }
    public MockUnitOfWork(IUserRepository mock)
    {
      userRepository = mock;
    }
    public MockUnitOfWork(ITodoRepository todo, IUserRepository user)
    {
        userRepository = user;
        todoRepository = todo;
    }
    public MockUnitOfWork()
    {}
    public ITodoRepository TodoRepository
    {
        get
        {
            if (todoRepository == null)
            {
                todoRepository = new Mock<ITodoRepository>().Object;
            }
            return todoRepository;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            if (userRepository == null)
            {
                userRepository = new Mock<IUserRepository>().Object;
            }
            return userRepository;
        }
    }

    public virtual Task<int> Save()
    {
        return Task.FromResult(0);
    }
}