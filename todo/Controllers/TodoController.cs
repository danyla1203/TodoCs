using Microsoft.AspNetCore.Mvc;
using todo.Models;
using todo.Repositories;

namespace todo.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly UnitOfWork _unit;

    public TodoController(UnitOfWork unit, ILogger<TodoController> logger)
    {
        _logger = logger;
        _unit = unit;

    }

    [HttpGet(Name = "GetTodoItems")]
    public IEnumerable<TodoItem> GetTodoItems()
    {
        return _unit.TodoRepository.GetAll();
    }

    [HttpGet("{id}", Name = "GetTodoById")]
    public ActionResult<TodoItem> GetTodoItem(int id)
    {
        TodoItem item = _unit.TodoRepository.GetById(id);
        if (item == null)
        {
            return NotFound();
        }
        return item;
    }


    [HttpPost(Name = "AddTodoItem")]
    public ActionResult<TodoItem> AddTodoItem(TodoItem todoItem)
    {
        _unit.TodoRepository.AddItem(todoItem);
        _unit.Save();
        
        return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
    }
}

