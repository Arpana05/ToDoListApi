using ToDoListApi.Models;
using ToDoListApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListApi.Controllers;

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
        var items = await _itemsService.GetAsync();
        return Ok(items);
    }


    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ReadItemDto>> Get(string id)
    {
        var item = await _itemsService.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }


    [HttpPost]
    public async Task<ActionResult<ReadItemDto>> Post(CreateItemDto newItem)
    {
        var createdItem = await _itemsService.CreateAsync(newItem);
        return CreatedAtAction(nameof(Get), new { id = createdItem.Id }, createdItem);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ReadItemDto>> Update(string id, UpdateItemDto updatedItem)
    {
        var updated = await _itemsService.UpdateAsync(id, updatedItem);
        if (updated is null)
        {
            return NotFound();
        }
        return Ok(updated);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await _itemsService.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }
        await _itemsService.RemoveAsync(id);
        return NoContent();
    }
}
