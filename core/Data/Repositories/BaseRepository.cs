using Microsoft.EntityFrameworkCore;

namespace todo.Data.Repositories;
public class BaseRepository<TEntity>: IRepositoryBase<TEntity>
    where TEntity : class
{
    protected DbSet<TEntity> _table;

    public BaseRepository(DbSet<TEntity> table)
    {
        _table = table;
    }

    public TEntity? GetById(int id) => _table.Find((long)id);
    public IEnumerable<TEntity> GetAll() => _table.ToList();
    public TEntity AddItem(TEntity item)
    {
        _table.Add(item);
        return item;
    }
    public TEntity? Delete(int id)
    {
        TEntity? record = _table.Find((long)id);
        if (record != null) _table.Remove(record);
        return record;
    }

}