using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spendly.Data;
using Spendly.Models;
using Spendly.ViewModels;

namespace Spendly.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var now = DateTime.Today;

            var allExpenses = await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalThisMonth = allExpenses
                    .Where(e => e.Date.Month == now.Month && e.Date.Year == now.Year)
                    .Sum(e => e.Amount),

                TotalLastMonth = allExpenses
                    .Where(e => e.Date.Month == now.AddMonths(-1).Month
                             && e.Date.Year == now.AddMonths(-1).Year)
                    .Sum(e => e.Amount),

                TotalThisYear = allExpenses
                    .Where(e => e.Date.Year == now.Year)
                    .Sum(e => e.Amount),

                TotalExpensesCount = allExpenses.Count
            };

            // Category breakdown for pie chart
            var byCategory = allExpenses
                .Where(e => e.Date.Month == now.Month && e.Date.Year == now.Year)
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Name = g.Key.Name,
                    Total = g.Sum(e => e.Amount),
                    Color = g.Key.ColorHex
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            vm.CategoryNames = byCategory.Select(x => x.Name).ToList();
            vm.CategoryTotals = byCategory.Select(x => x.Total).ToList();
            vm.CategoryColors = byCategory.Select(x => x.Color).ToList();

            // Last 6 months for bar chart
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                vm.MonthLabels.Add(month.ToString("MMM yyyy"));
                vm.MonthTotals.Add(allExpenses
                    .Where(e => e.Date.Month == month.Month
                             && e.Date.Year == month.Year)
                    .Sum(e => e.Amount));
            }

            // Recent 5 expenses
            vm.RecentExpenses = allExpenses
                .OrderByDescending(e => e.Date)
                .Take(5)
                .Select(e => new RecentExpenseItem
                {
                    Title = e.Title,
                    Amount = e.Amount,
                    CategoryName = e.Category.Name,
                    CategoryColor = e.Category.ColorHex,
                    Date = e.Date
                }).ToList();

            return View(vm);
        }
    }
}