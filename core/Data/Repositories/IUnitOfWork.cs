using todo.Data.Repositories;

public interface IUnitOfWork
{
  Task<int> Save();
  ITodoRepository TodoRepository { get; }

  IUserRepository UserRepository { get; }
}