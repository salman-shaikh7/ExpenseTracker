using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data

{
    public class ApplicationDbContext : DbContext

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
        {
            
        }

        public DbSet <Expense> expenses { get; set; }
        public DbSet <PaymentMethod> paymentMethods { get; set; }
        public DbSet <Category> categories { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category { CategoryId = 1, Name = "Food" },
        //        new Category { CategoryId = 1, Name = "Shopping" },
        //        new Category { CategoryId = 1, Name = "Fuel" },
        //        new Category { CategoryId = 1, Name = "Rent" }

        //        );

        //}
    }
}
