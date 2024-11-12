namespace ExpenseTracker.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        // One-to-Many relationship with Expense
        public ICollection<Expense> Expenses { get; set; }
    }
}
