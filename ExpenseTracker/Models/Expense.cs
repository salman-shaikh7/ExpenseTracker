using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models
{

    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        public string? Description { get; set; }

        // Foreign Key for Category
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        // Foreign Key for PaymentMethod
        public int? PaymentMethodId { get; set; } 
        public PaymentMethod? PaymentMethod { get; set; }
    }


}
