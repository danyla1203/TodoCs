using System.Linq.Expressions;

namespace todo.Data.Repositories;

public interface IRepositoryBase<T> where T : class
{
    List<T> GetAll(
        Expression<Func<T, bool>>? filter = null
    );
    T? GetById(int id);
    T AddItem(T item);
    T? Delete(int id);
}