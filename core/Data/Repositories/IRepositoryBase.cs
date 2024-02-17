namespace todo.Data.Repositories;

public interface IRepositoryBase<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(int id);
    T AddItem(T item);
    T? Delete(int id);
}