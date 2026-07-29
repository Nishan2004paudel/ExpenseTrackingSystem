using System.Security.Claims;
using expensetrackerserver.DTOs;
using expensetrackerserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var category = await _service.Create(dto, userId);
            return Ok(category);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var categories = await _service.GetAllByUserId(userId);
            return Ok(categories);
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> Update(int categoryId, UpdateCategoryDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var category = await _service.Update(categoryId, dto, userId);
            return Ok(category);
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> Delete(int categoryId, [FromBody] DeleteCategoryDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.Delete(categoryId, dto, userId);
            if (result != null)
            {
                return Ok(result);
            }
            return NoContent();
        }
    }
}
