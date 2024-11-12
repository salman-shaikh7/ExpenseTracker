namespace ExpenseTracker.Models
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string MethodName { get; set; }

        // One-to-Many relationship with Expense
        public ICollection<Expense> Expenses { get; set; }
    }
}
