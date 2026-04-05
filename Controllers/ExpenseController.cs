using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spendly.Data;
using Spendly.Models;
using Spendly.Repositories;
using Spendly.Services;
using Spendly.ViewModels;

namespace Spendly.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IExpenseRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ExportService _exportService;

        public ExpenseController(
            IExpenseRepository repo,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ExportService exportService)
        {
            _repo = repo;
            _userManager = userManager;
            _context = context;
            _exportService = exportService;
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        // GET: /Expense
        public async Task<IActionResult> Index(
            int? categoryId,
            DateTime? from,
            DateTime? to,
            string? keyword)
        {
            var userId = _userManager.GetUserId(User)!;

            var expenses = await _repo.FilterAsync(
                userId, categoryId, from, to, keyword);

            ViewBag.Categories = await GetCategoriesAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");
            ViewBag.Keyword = keyword;
            ViewBag.Total = expenses.Sum(e => e.Amount);

            return View(expenses);
        }

        // GET: /Expense/Create
        public async Task<IActionResult> Create()
        {
            var vm = new ExpenseViewModel
            {
                Date = DateTime.Today,
                Categories = await GetCategoriesAsync()
            };

            return View(vm);
        }

        // POST: /Expense/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await GetCategoriesAsync();
                return View(vm);
            }

            var expense = new Expense
            {
                Title = vm.Title,
                Amount = vm.Amount,
                Date = vm.Date,
                Notes = vm.Notes,
                CategoryId = vm.CategoryId,
                UserId = _userManager.GetUserId(User)!
            };

            await _repo.AddAsync(expense);

            TempData["Success"] = "Expense added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Expense/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var expense = await _repo.GetByIdAsync(id, userId);

            if (expense == null)
                return NotFound();

            var vm = new ExpenseViewModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                Notes = expense.Notes,
                CategoryId = expense.CategoryId,
                Categories = await GetCategoriesAsync()
            };

            return View(vm);
        }

        // POST: /Expense/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpenseViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = await GetCategoriesAsync();
                return View(vm);
            }

            var userId = _userManager.GetUserId(User)!;
            var expense = await _repo.GetByIdAsync(vm.Id, userId);

            if (expense == null)
                return NotFound();

            expense.Title = vm.Title;
            expense.Amount = vm.Amount;
            expense.Date = vm.Date;
            expense.Notes = vm.Notes;
            expense.CategoryId = vm.CategoryId;

            await _repo.UpdateAsync(expense);

            TempData["Success"] = "Expense updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Expense/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User)!;

            await _repo.DeleteAsync(id, userId);

            TempData["Success"] = "Expense deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Export CSV
        public async Task<IActionResult> Export(
            int? categoryId,
            DateTime? from,
            DateTime? to,
            string? keyword)
        {
            var userId = _userManager.GetUserId(User)!;

            var expenses = await _repo.FilterAsync(
                userId, categoryId, from, to, keyword);

            var csvBytes = _exportService.ExportExpensesToCsv(expenses);

            var fileName = $"Spendly_Expenses_{DateTime.Today:yyyyMMdd}.csv";

            return File(csvBytes, "text/csv", fileName);
        }
    }
}