namespace todo.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    private ITodoRepository _todoRepository;
    private IUserRepository _userRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        _todoRepository = new TodoRepository(context);
        _userRepository = new UserRepository(context);
    }

    public ITodoRepository TodoRepository
    {
        get
        {
            return _todoRepository;
        }
         
    }
    public IUserRepository UserRepository
    {
        get
        {
            return _userRepository;
        }
    }

    public async Task<int> Save()
    {
        return await _context.SaveChangesAsync();
    }
}