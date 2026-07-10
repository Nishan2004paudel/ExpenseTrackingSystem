using System.Security.Claims;
using expensetrackerserver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace expensetrackerserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] int year,
            [FromQuery] int? month,
            [FromQuery] int? categoryId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var summary = await _service.GetSummary(
                userId,
                year,
                month,
                categoryId);

            return Ok(summary);
        }

        [HttpGet("months")]
        public async Task<IActionResult> GetExpenseByMonth([FromQuery] int year)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.GetExpenseByMonth(userId, year);

            return Ok(result);
        }
        [HttpGet("month/breakdown")]
        public async Task<IActionResult> GetMonthBreakdown([FromQuery] int year, [FromQuery] int month, [FromQuery] bool includeEmpty = false)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.GetMonthBreakdown(userId, year, month, includeEmpty);

            return Ok(result);
        }
        [HttpGet("categories")]
        public async Task<IActionResult> GetExpenseByCategory([FromQuery] int year, [FromQuery] bool includeEmpty = false)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.GetExpenseByCategory(userId, year, includeEmpty);

            return Ok(result);
        }

        [HttpGet("categories/breakdown")]
        public async Task<IActionResult> GetCategoryBreakdown(
            [FromQuery] int year,
            [FromQuery] int categoryId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _service.GetCategoryBreakdown(
                userId,
                year,
                categoryId);

            return Ok(result);
        }
    }
}
