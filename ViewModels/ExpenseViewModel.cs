using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Spendly.ViewModels
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [DataType(DataType.Currency)]
        [Display(Name = "Amount (৳)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [StringLength(300)]
        public string? Notes { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Populated from DB for the dropdown
        public IEnumerable<SelectListItem>? Categories { get; set; }

        // Display only
        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
    }
}