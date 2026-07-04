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
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _service;
        public ExpenseController(IExpenseService service)
        {
            _service = service;
        }
        //complete this

        [HttpPost]
        public async Task<IActionResult> Create(CreateExpenseDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expense = await _service.Create(dto, userId);
            return Ok(expense);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expenes = await _service.GetAllByUserId(userId);
            return Ok(expenes);
        }

        [HttpGet("{expenseId}")]
        public async Task<IActionResult> GetById(int expenseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var expense = await _service.GetById(expenseId, userId);

            return Ok(expense);
        }

        [HttpPut("{expenseId}")]
        public async Task<IActionResult> Update(int expenseId, UpdateExpenseDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var expense = await _service.Update(expenseId, dto, userId);
            return Ok(expense);
        }

        [HttpDelete("{expenseId}")]
        public async Task<IActionResult> Delete(int expenseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _service.SoftDelete(expenseId, userId);
            return NoContent();
        }
    }
}
