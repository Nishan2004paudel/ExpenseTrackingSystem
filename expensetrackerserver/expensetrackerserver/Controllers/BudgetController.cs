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
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _service;
        public BudgetController(IBudgetService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBudgetDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var budget = await _service.Create(dto, userId);
            return Ok(budget);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var budgets = await _service.GetAllByUserId(userId);
            return Ok(budgets);
        }

        [HttpGet("{budgetId}")]
        public async Task<IActionResult> GetById(int budgetId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var budget = await _service.GetById(budgetId, userId);

            return Ok(budget);
        }

        [HttpPut("{budgetId}")]
        public async Task<IActionResult> Update(int budgetId, UpdateBudgetDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var budget = await _service.Update(budgetId, dto, userId);
            return Ok(budget);
        }

        [HttpDelete("{budgetId}")]
        public async Task<IActionResult> Delete(int budgetId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.SoftDelete(budgetId, userId);
            return NoContent();
        }
    }
}
