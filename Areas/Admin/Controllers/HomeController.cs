using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spendly.Areas.Admin.Models;
using Spendly.Data;
using Spendly.Models;

namespace Spendly.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .ToListAsync();

            var vm = new AdminDashboardViewModel
            {
                TotalUsers = users.Count,
                TotalExpenses = expenses.Count,
                TotalAmountAllUsers = expenses.Sum(e => e.Amount),
                UserSummaries = users.Select(u => new UserSummary
                {
                    UserId = u.Id,
                    FullName = u.FullName ?? "N/A",
                    Email = u.Email!,
                    ExpenseCount = expenses.Count(e => e.UserId == u.Id),
                    TotalSpent = expenses
                        .Where(e => e.UserId == u.Id)
                        .Sum(e => e.Amount),
                    LastExpenseDate = expenses
                        .Where(e => e.UserId == u.Id)
                        .OrderByDescending(e => e.Date)
                        .FirstOrDefault()?.Date
                }).OrderByDescending(u => u.TotalSpent).ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> UserExpenses(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var expenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            ViewBag.UserName = user.FullName ?? user.Email;
            ViewBag.UserEmail = user.Email;
            return View(expenses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Delete user's expenses first
            var expenses = _context.Expenses.Where(e => e.UserId == userId);
            _context.Expenses.RemoveRange(expenses);
            await _context.SaveChangesAsync();

            await _userManager.DeleteAsync(user);
            TempData["Success"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}