using Microsoft.AspNetCore.Mvc;
using todo.Data.Dto;
using todo.Services;

namespace todo.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<TodoController> _logger;
    private readonly IUserService _service;
    public UserController(
        IUserService service,
        ILogger<TodoController> logger
    )
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet("{id}", Name = "GetUserProfile")]
    [Produces("application/json")]
    public async Task<ActionResult<CreatedUserDto>> GetUserProfile(int id)
    {
        _logger.LogInformation($"Get user profile. User id: {id}");
        return await _service.GetUser(id);
    }

    [HttpPost(Name = "CreateUser")]
    [Produces("application/json")]
    public async Task<ActionResult<CreatedUserDto>> CreateUser(AddUserDto user)
    {
        _logger.LogInformation($"Create user. Data: {@user}", user);
        var newUser = await _service.CreateUser(user);
        return CreatedAtAction(
            nameof(GetUserProfile),
            new { id = newUser.Id },
            newUser
        );
    }
}