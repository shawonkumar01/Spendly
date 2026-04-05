namespace Spendly.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalExpenses { get; set; }
        public decimal TotalAmountAllUsers { get; set; }
        public List<UserSummary> UserSummaries { get; set; } = new();
    }

    public class UserSummary
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int ExpenseCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastExpenseDate { get; set; }
    }
}