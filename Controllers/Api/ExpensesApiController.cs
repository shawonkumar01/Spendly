using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spendly.Models;
using Spendly.Repositories;

namespace Spendly.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesApiController : ControllerBase
    {
        private readonly IExpenseRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpensesApiController(
            IExpenseRepository repo,
            UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        // GET /api/expenses
        [HttpGet]
        public async Task<IActionResult> GetAll(
            int? categoryId, DateTime? from, DateTime? to, string? keyword)
        {
            var userId = _userManager.GetUserId(User)!;

            var expenses = await _repo.FilterAsync(
                userId, categoryId, from, to, keyword);

            var result = expenses.Select(e => new
            {
                e.Id,
                e.Title,
                e.Amount,
                Date = e.Date.ToString("yyyy-MM-dd"),
                e.Notes,
                Category = e.Category.Name,
                CategoryColor = e.Category.ColorHex
            });

            return Ok(result);
        }

        // GET /api/expenses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var expense = await _repo.GetByIdAsync(id, userId);

            if (expense == null) return NotFound(new { message = "Expense not found." });

            return Ok(new
            {
                expense.Id,
                expense.Title,
                expense.Amount,
                Date = expense.Date.ToString("yyyy-MM-dd"),
                expense.Notes,
                Category = expense.Category.Name,
                CategoryColor = expense.Category.ColorHex
            });
        }

        // POST /api/expenses
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiExpenseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User)!;

            var expense = new Expense
            {
                Title = request.Title,
                Amount = request.Amount,
                Date = request.Date,
                Notes = request.Notes,
                CategoryId = request.CategoryId,
                UserId = userId
            };

            await _repo.AddAsync(expense);

            return CreatedAtAction(nameof(GetById),
                new { id = expense.Id },
                new { expense.Id, expense.Title, expense.Amount });
        }

        // PUT /api/expenses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] ApiExpenseRequest request)
        {
            var userId = _userManager.GetUserId(User)!;
            var expense = await _repo.GetByIdAsync(id, userId);

            if (expense == null) return NotFound(new { message = "Expense not found." });

            expense.Title = request.Title;
            expense.Amount = request.Amount;
            expense.Date = request.Date;
            expense.Notes = request.Notes;
            expense.CategoryId = request.CategoryId;

            await _repo.UpdateAsync(expense);
            return Ok(new { message = "Updated successfully." });
        }

        // DELETE /api/expenses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var expense = await _repo.GetByIdAsync(id, userId);

            if (expense == null) return NotFound(new { message = "Expense not found." });

            await _repo.DeleteAsync(id, userId);
            return Ok(new { message = "Deleted successfully." });
        }
    }

    public class ApiExpenseRequest
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public int CategoryId { get; set; }
    }
}