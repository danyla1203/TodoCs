using System.Linq.Expressions;

namespace todo.Data.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task<List<T>> GetAll(
        Expression<Func<T, bool>>? filter = null
    );
    Task<T?> GetById(int id);
    Task<T> AddItem(T item);
    Task<T?> Delete(int id);
}