using Moq;
using todo.Data.Repositories;

namespace Todo.Tests.Utils.Mock;

public class MockUnitOfWork : IUnitOfWork
{
  ITodoRepository todoRepository;
  public MockUnitOfWork(ITodoRepository mock)
  {
    todoRepository = mock;
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
  public virtual void Save()
  {}
}