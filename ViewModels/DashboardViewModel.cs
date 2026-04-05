namespace Spendly.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalThisMonth { get; set; }
        public decimal TotalLastMonth { get; set; }
        public decimal TotalThisYear { get; set; }
        public int TotalExpensesCount { get; set; }

        // For the pie chart — category breakdown
        public List<string> CategoryNames { get; set; } = new();
        public List<decimal> CategoryTotals { get; set; } = new();
        public List<string> CategoryColors { get; set; } = new();

        // For the bar chart — last 6 months spending
        public List<string> MonthLabels { get; set; } = new();
        public List<decimal> MonthTotals { get; set; } = new();

        // Recent expenses
        public List<RecentExpenseItem> RecentExpenses { get; set; } = new();
    }

    public class RecentExpenseItem
    {
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime Date { get; set; }
    }
}