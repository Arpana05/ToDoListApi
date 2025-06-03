using ToDoListApi.Models;
using ToDoListApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ToDoListApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService) =>
        _usersService = usersService;


    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _usersService.LoginAsync(dto);
        return Ok(new { Token = token });
    }


    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _usersService.RegisterAsync(dto);
        return Ok(user);
    }


    [HttpGet]
    public async Task<ActionResult<ReadUserDto>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId)) return Unauthorized("User ID not found.");

        var user = await _usersService.GetAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

//     [HttpGet]
// public async Task<ActionResult<ReadUserDto>> Get()
// {
//     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//     if (string.IsNullOrEmpty(userId)) return Unauthorized();

//     var userWithItems = await _usersService.GetAsync(userId);
//     if (userWithItems == null) return NotFound();

//     return Ok(userWithItems);
// }


    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ReadUserDto>> UpdateUser(string id, UpdateUserDto dto)
    {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (loggedInUserId != id)
        {
            return Forbid();
        }

        var updatedUser = await _usersService.UpdateAsync(id, dto);
        if (updatedUser == null) return NotFound();
        {
            return Ok(updatedUser);
        }
    }



    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (loggedInUserId != id)
        {
            return Forbid();
        }

        var user = await _usersService.GetAsync(id);
        if (user == null) return NotFound();

        await _usersService.RemoveAsync(id);
        return NoContent();
    }

}
