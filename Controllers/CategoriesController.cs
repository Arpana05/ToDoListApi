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
    public async Task<List<Category>> Get() =>
        await _categoriesService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Category>> Get(string id)
    {
        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        return category;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Category newCategory)
    {
        await _categoriesService.CreateAsync(newCategory);

        return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Category updatedCategory)
    {
        var category = await _categoriesService.GetAsync(id);

        if (category is null)
        {
            return NotFound();
        }

        updatedCategory.Id = category.Id;

        await _categoriesService.UpdateAsync(id, updatedCategory);

        return NoContent();
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
