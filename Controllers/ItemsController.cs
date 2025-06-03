using ToDoListApi.DTOs;
using ToDoListApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ToDoListApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly ItemsService _itemsService;

    public ItemsController(ItemsService itemsService)
    {
        _itemsService = itemsService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ReadItemDto>>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var items = await _itemsService.GetAsync(userId!);
        return Ok(items);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ReadItemDto>> Get(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var item = await _itemsService.GetByIdAsync(id, userId!);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }



    [HttpPost]
    public async Task<ActionResult<ReadItemDto>> Post(CreateItemDto newItem)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
        return Unauthorized("User ID not found in token.");

    var createdItem = await _itemsService.CreateAsync(newItem, userId);
    return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
}



    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ReadItemDto>> Update(string id, UpdateItemDto updatedItem)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var updated = await _itemsService.UpdateAsync(id, updatedItem, userId!);
        if (updated is null)
        {
            return NotFound();
        }
        return Ok(updated);
    }


    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = await _itemsService.GetByIdAsync(id, userId!);
        if (existing is null)
        {
            return NotFound();
        }
        var success = await _itemsService.RemoveAsync(id, userId!);
        if (!success) return Forbid();

        return NoContent();
    }

}
