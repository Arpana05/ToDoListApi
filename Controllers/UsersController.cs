using ToDoListApi.Models;
using ToDoListApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService) =>
        _usersService = usersService;

    [HttpGet]
    public async Task<ActionResult<List<ReadUserDto>>> Get()
    {
        var users = await _usersService.GetAsync();
        return Ok(users);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ReadUserDto>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<ReadUserDto>> Post(CreateUserDto newUserDto)
    {
        var createdUser = await _usersService.CreateAsync(newUserDto);
        return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, UpdateUserDto updatedUserDto)
    {
        var updatedUser = await _usersService.UpdateAsync(id, updatedUserDto);

        if (updatedUser is null)
        {
            return NotFound();
        }

        return Ok(updatedUser);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _usersService.RemoveAsync(id);
        return NoContent();
    }
}
