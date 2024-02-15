using todo.Models;
namespace todo.Repositories;

public class UnitOfWork
{
    private ApplicationDbContext _context;
    private TodoRepository _todoRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _todoRepository = new TodoRepository(context.TodoItems);
    }

    public TodoRepository TodoRepository
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