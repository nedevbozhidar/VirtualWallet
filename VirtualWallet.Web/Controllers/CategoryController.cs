using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Business.DTOs.Categories;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;


namespace VirtualWallet.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    
    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        return Ok(await _categoryService.GetUserCategoriesAsync(userId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        
        try
        {
            return Ok(await _categoryService.GetCategoryByIdAsync(userId, id));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryCreateDto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            CategoryResponseDto category = await _categoryService.CreateCategoryAsync(userId, categoryCreateDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new  { message = ex.Message });
        }
        catch (DuplicateEntityException ex) 
        {
            return Conflict(new  { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto categoryUpdateDto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        try
        {
            return Ok(await _categoryService.UpdateCategoryAsync(userId, id, categoryUpdateDto));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedOperationException)
        {
            return Forbid();
        }
        catch (DuplicateEntityException ex)
        {
            return Conflict(new  { message = ex.Message });
        }
    }
    
    private bool TryGetUserId(out int userId)
    {
        userId = 0;
        return User.Identity?.IsAuthenticated == true
               && int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}