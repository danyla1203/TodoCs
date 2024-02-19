using Microsoft.AspNetCore.Mvc;
using todo.Data.Dto;
using todo.Models;
using todo.Services;

namespace todo.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly ITodoService _service;
    public TodoController(ITodoService service, ILogger<TodoController> logger)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet(Name = "GetTodoItems")]
    [Produces("application/json")]
    public TodoListDto GetTodoItems(bool? completed) => _service.GetTodoList(completed);

    [HttpGet("{id}", Name = "GetTodoById")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoItem> GetTodoItem(int id)
    {
        TodoItem? record = _service.GetTodoItem(id);
        if (record == null)
        {
            return NotFound();
        }
        return record;
    }

    [HttpPost(Name = "AddTodoItem")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TodoItem> AddTodoItem(TodoItem todoItem)
    {
        TodoItem newRecord = _service.AddTodoItem(todoItem);
        return CreatedAtAction(nameof(GetTodoItem), new { id = newRecord.Id }, newRecord);
    }

    [HttpDelete(Name = "DeleteTodoItem")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoItem> DeleteTodoItem(int id)
    {
        TodoItem? deleted = _service.DeleteTodoItem(id);
        if (deleted == null)
        {
            return NotFound();
        }
        return deleted;
    }
}

