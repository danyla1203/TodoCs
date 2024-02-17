using todo.Data.Repositories;

public interface IUnitOfWork
{
  void Save();
  ITodoRepository TodoRepository { get; }
}