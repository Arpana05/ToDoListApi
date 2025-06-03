using ToDoListApi.DTOs;
using ToDoListApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ToDoListApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoriesService _categoriesService;

    public CategoriesController(CategoriesService categoriesService) =>
        _categoriesService = categoriesService;



    [HttpGet]
    public async Task<ActionResult<List<ReadCategoryDto>>> Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var categories = await _categoriesService.GetAsync(userId!);
        return Ok(categories);
    }



    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ReadCategoryDto>> Get(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await _categoriesService.GetAsync(id, userId!);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }
    



    [HttpPost]
    public async Task<ActionResult<ReadCategoryDto>> Post(CreateCategoryDto newCategoryDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var createdCategory = await _categoriesService.CreateAsync(newCategoryDto, userId!);
        return CreatedAtAction(nameof(Get), new { id = createdCategory.Id }, createdCategory);
    }



    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ReadCategoryDto>> Update(string id, UpdateCategoryDto updateDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var updatedCategory = await _categoriesService.UpdateAsync(id, updateDto, userId!);
        if (updatedCategory is null) return NotFound();
        return Ok(updatedCategory);
    }



    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var category = await _categoriesService.GetAsync(id, userId!);
        if (category is null) return NotFound();
        await _categoriesService.RemoveAsync(id, userId!);
        return NoContent();
    }
}
