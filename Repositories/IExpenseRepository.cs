using Spendly.Models;

namespace Spendly.Repositories
{
    public interface IExpenseRepository
    {
        Task<IEnumerable<Expense>> GetAllByUserAsync(string userId);
        Task<Expense?> GetByIdAsync(int id, string userId);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id, string userId);
        Task<IEnumerable<Expense>> FilterAsync(string userId,
            int? categoryId, DateTime? from, DateTime? to, string? keyword);
    }
}