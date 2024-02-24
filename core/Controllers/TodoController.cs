using AutoMapper;
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
    public async Task<TodoListDto> GetTodoItems(bool? completed)
    {
        _logger.LogInformation($"Get Todo list. Filtering: completed: {completed}");
        List<TodoItem> list = await _service.GetTodoList(completed);
        return new TodoListDto
        {
            count = list.Count,
            items = list
        };
    }

    [HttpGet("{id}", Name = "GetTodoById")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<TodoItem> GetTodoItem(int id)
    {
        _logger.LogInformation($"Get Todo by id. Id: {id}");
        return await _service.GetTodoItem(id);
    }

    [HttpPost(Name = "AddTodoItem")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<TodoItem> AddTodoItem(TodoItemDto todoItem)
    {
        _logger.LogInformation("Add Todo item. Data: {@todoItem}", todoItem);
        return await _service.AddTodoItem(todoItem);
    }

    [HttpDelete("{id}", Name = "DeleteTodoItem")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<TodoItem> DeleteTodoItem(int id)
    {
        _logger.LogInformation($"Delete Todo item. Id: {id}");
        return await _service.DeleteTodoItem(id);
    }
}

