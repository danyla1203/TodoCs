using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace todo.Data.Repositories;
public class BaseRepository<TEntity> : IRepositoryBase<TEntity>
    where TEntity : class
{
    protected DbSet<TEntity> _table;

    public BaseRepository(DbSet<TEntity> table)
    {
        _table = table;
    }

    public async Task<TEntity?> GetById(int id)
    {
        return await _table.FindAsync((long)id);
    }
    public async virtual Task<List<TEntity>> GetAll(
        Expression<Func<TEntity, bool>>? filter = null
    )
    {
        IQueryable<TEntity> query = _table;
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync();
    }
    public async Task<TEntity> AddItem(TEntity item)
    {
        await _table.AddAsync(item);
        return item;
    }
    public async Task<TEntity?> Delete(int id)
    {
        TEntity? record = await _table.FindAsync((long)id);
        if (record != null) _table.Remove(record);
        return record;
    }

}