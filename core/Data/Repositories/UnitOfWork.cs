﻿namespace todo.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    private ITodoRepository _todoRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _todoRepository = new TodoRepository(context);
    }

    public ITodoRepository TodoRepository
    {
        get
        {
            return _todoRepository;
        }
         
    }
    public async Task<int> Save()
    {
        return await _context.SaveChangesAsync();
    }
}