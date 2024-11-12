namespace ExpenseTracker.Models
{
    public class ExpenseOverviewModel
    {
        public IEnumerable<dynamic> CategoryData { get; set; }
        public IEnumerable<dynamic> DateData { get; set; }
        public IEnumerable<dynamic> PaymentMethodData { get; set; }
    }
}
