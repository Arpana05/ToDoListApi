using ToDoListApi.Models;
using ToDoListApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListApi.Controllers;

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
        var categories = await _categoriesService.GetAsync();
        return Ok(categories);
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<ReadCategoryDto>> Get(string id)
    {
        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<ReadCategoryDto>> Post(CreateCategoryDto newCategoryDto)
    {
        var createdCategory = await _categoriesService.CreateAsync(newCategoryDto);

        return CreatedAtAction(nameof(Get), new { id = createdCategory.Id }, createdCategory);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult<ReadCategoryDto>> Update(string id, UpdateCategoryDto updateDto)
    {
        var updatedCategory = await _categoriesService.UpdateAsync(id, updateDto);

        if (updatedCategory is null)
        {
            return NotFound();
        }

        return Ok(updatedCategory);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        await _categoriesService.RemoveAsync(id);

        return NoContent();
    }
}
