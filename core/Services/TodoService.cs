using todo.Models;

namespace todo.Services;

public class TodoService : ITodoService
{
    private readonly IUnitOfWork _unit;

    public TodoService(IUnitOfWork unit)
    {
        _unit = unit;
    }
    
    public IEnumerable<TodoItem> GetTodoList()
    {
        return _unit.TodoRepository.GetAll();
    }

    public TodoItem? GetTodoItem(int id)
    {
       return _unit.TodoRepository.GetById(id); 
    }
    public TodoItem AddTodoItem(TodoItem todoItem)
    {
        _unit.TodoRepository.AddItem(todoItem);
        _unit.Save();
        return todoItem;
    }
    public TodoItem? DeleteTodoItem(int id)
    {
        TodoItem? record = _unit.TodoRepository.GetById(id);
        if (record != null)
        {
            _unit.TodoRepository.Delete(id);
            _unit.Save();
        }
        return record;
    }
}