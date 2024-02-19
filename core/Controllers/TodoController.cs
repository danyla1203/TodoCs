using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo.Data.Dto;
using todo.Models;
using todo.Services;

namespace todo.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IMapper _mapper;
    private readonly ITodoService _service;
    public TodoController(
        IMapper mapper,
        ITodoService service,
        ILogger<TodoController> logger
    )
    {
        _logger = logger;
        _service = service;
        _mapper = mapper;
    }

    [HttpGet(Name = "GetTodoItems")]
    [Produces("application/json")]
    public TodoListDto GetTodoItems(bool? completed)
    {
        List<TodoItem> items = _service.GetTodoList(completed);
        return new TodoListDto
        {
            count = items.Count,
            items = _mapper.Map<List<TodoItem>, List<TodoItemDto>>(items)
        };
    }

    [HttpGet("{id}", Name = "GetTodoById")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoItemDto> GetTodoItem(int id)
    {
        TodoItem? record = _service.GetTodoItem(id);
        return record != null ?
            _mapper.Map<TodoItem, TodoItemDto>(record) :
            NotFound("Todo item not found");
    }

    [HttpPost(Name = "AddTodoItem")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TodoItemDto> AddTodoItem(TodoItem todoItem)
    {
        try
        {
            TodoItem newRecord = _service.AddTodoItem(todoItem);
            return CreatedAtAction(
                nameof(GetTodoItem),
                new { id = newRecord.Id },
                newRecord
            );
        }
        catch (DbUpdateException)
        {
            return StatusCode(500);
        }
    }

    [HttpDelete(Name = "DeleteTodoItem")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TodoItemDto> DeleteTodoItem(int id)
    {
        try
        {
            TodoItem? deleted = _service.DeleteTodoItem(id);
            return deleted != null ?
                _mapper.Map<TodoItem, TodoItemDto>(deleted) :
                NotFound("Todo item not found");
        }
        catch (DbUpdateException)
        {
            return StatusCode(500);
        }
    }
}

