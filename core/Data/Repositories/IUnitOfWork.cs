using todo.Data.Repositories;

public interface IUnitOfWork
{
  Task<int> Save();
  ITodoRepository TodoRepository { get; }
}