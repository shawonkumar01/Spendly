using System.ComponentModel.DataAnnotations;

namespace Spendly.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string? ColorHex { get; set; } = "#6c757d";

        public ICollection<Expense> Expenses { get; set; }
    }
}