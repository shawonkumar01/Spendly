using Microsoft.EntityFrameworkCore;
using Spendly.Data;
using Spendly.Models;

namespace Spendly.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ApplicationDbContext _context;

        public ExpenseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetAllByUserAsync(string userId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<Expense?> GetByIdAsync(int id, string userId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
        }

        public async Task AddAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var expense = await GetByIdAsync(id, userId);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Expense>> FilterAsync(string userId,
            int? categoryId, DateTime? from, DateTime? to, string? keyword)
        {
            var query = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId);

            if (from.HasValue)
                query = query.Where(e => e.Date >= from);

            if (to.HasValue)
                query = query.Where(e => e.Date <= to);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(e =>
                    e.Title.Contains(keyword) ||
                    (e.Notes != null && e.Notes.Contains(keyword)));

            return await query.OrderByDescending(e => e.Date).ToListAsync();
        }
    }
}