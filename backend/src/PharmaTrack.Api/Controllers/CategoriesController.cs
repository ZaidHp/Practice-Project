using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmaTrack.Application.DTOs.Category;
using PharmaTrack.Application.Interfaces;

namespace PharmaTrack.Api.Controllers;

[Authorize(Roles = "Admin, Store Manager")]
[ApiController]
[Route("api/[Controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var response = await _categoryService.GetAllCategoryAsync(page, pageSize, search);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var username = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var response = await _categoryService.CreateCategoryAsync(dto, username);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("id")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await _categoryService.DeleteCategoryAsync(id);

        if (!response.Success)
            return NotFound(response);
        
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var username = User.FindFirstValue(ClaimTypes.Name) ?? "System";
        var response = await _categoryService.UpdateCategoryAsync(id, dto, username);

        if (!response.Success)
        {
            if (response.Message == "Category not found.")
                return NotFound(response);
            
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);

        if (!response.Success)
            return NotFound(response);
        
        return Ok(response);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCategories()
    {
        var response = await _categoryService.GetAllCategoriesAsync();
        return Ok(response);
    }
}