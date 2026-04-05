using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spendly.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Spendly.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasColumnType("decimal(18,2)");

            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Food", ColorHex = "#ef4444" },
                new Category { Id = 2, Name = "Transport", ColorHex = "#f97316" },
                new Category { Id = 3, Name = "Shopping", ColorHex = "#8b5cf6" },
                new Category { Id = 4, Name = "Bills", ColorHex = "#3b82f6" },
                new Category { Id = 5, Name = "Health", ColorHex = "#10b981" },
                new Category { Id = 6, Name = "Entertainment", ColorHex = "#f59e0b" },
                new Category { Id = 7, Name = "Other", ColorHex = "#6b7280" }
            );
        }
    }
}