namespace todo.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    private ITodoRepository _todoRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _todoRepository = new TodoRepository(context.TodoItems);
    }

    public ITodoRepository TodoRepository
    {
        get
        {
            return _todoRepository;
        }
         
    }
    public void Save()
    {
        _context.SaveChanges();
    }
}