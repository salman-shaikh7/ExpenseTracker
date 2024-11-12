using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;

using System.Linq;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ExpenseController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Create()
        {
            // Populate ViewBag for dropdowns
            ViewBag.Categories = _db.categories.ToList();
            ViewBag.PaymentMethods = _db.paymentMethods.ToList();

            return View(new Expense());
        }

        [HttpPost]
        public IActionResult Create(Expense expense)
        {
            // Repopulate ViewBag for dropdowns in case of validation errors
            ViewBag.Categories = _db.categories.ToList();
            ViewBag.PaymentMethods = _db.paymentMethods.ToList();

            if (ModelState.IsValid)
            {
                // Optionally attach related entities if necessary (not strictly required if only saving by ID)
                var category = _db.categories.FirstOrDefault(c => c.CategoryId == expense.CategoryId);
                if (category != null)
                {
                    expense.Category = category;
                }

                var paymentMethod = _db.paymentMethods.FirstOrDefault(pm => pm.PaymentMethodId == expense.PaymentMethodId);
                if (paymentMethod != null)
                {
                    expense.PaymentMethod = paymentMethod;
                }

                // Add the Expense to the database
                _db.expenses.Add(expense);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(expense);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Fetch the expense including the related Category and PaymentMethod
            var expense = _db.expenses.Include(e => e.Category).Include(e => e.PaymentMethod)
                .FirstOrDefault(e => e.ExpenseId == id);

            if (expense == null)
            {
                return NotFound();
            }

            // Get all categories and payment methods to show in dropdowns
            ViewData["Categories"] = new SelectList(_db.categories, "CategoryId", "Name", expense.CategoryId);
            ViewData["PaymentMethods"] = new SelectList(_db.paymentMethods, "PaymentMethodId", "MethodName", expense.PaymentMethodId);

            return View(expense);
        }

        [HttpPost]
        public IActionResult Edit(Expense expense)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing record to update
                var existingExpense = _db.expenses.Include(e => e.Category).Include(e => e.PaymentMethod)
                    .FirstOrDefault(e => e.ExpenseId == expense.ExpenseId);

                if (existingExpense == null)
                {
                    return NotFound();
                }

                // Update the properties of the existing record
                existingExpense.Description = expense.Description;
                existingExpense.Amount = expense.Amount;
                existingExpense.Date = expense.Date;
                existingExpense.CategoryId = expense.CategoryId;  // Ensure category is linked correctly
                existingExpense.PaymentMethodId = expense.PaymentMethodId;  // Ensure payment method is linked correctly

                // Save changes to the database
                _db.SaveChanges();

                TempData["success"] = "Expense updated successfully";
                return RedirectToAction("Index");
            }

            // If the model is invalid, reload categories and payment methods for the view
            ViewData["Categories"] = new SelectList(_db.categories, "CategoryId", "Name", expense.CategoryId);
            ViewData["PaymentMethods"] = new SelectList(_db.paymentMethods, "PaymentMethodId", "MethodName", expense.PaymentMethodId);

            return View(expense);  // Return to the edit view with the invalid model
        }



        // GET: Delete action - Display the expense details to confirm deletion
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // Fetch the expense including any related data (e.g., Category and PaymentMethod)
            Expense? obj = _db.expenses.Include(e => e.Category).Include(e => e.PaymentMethod).FirstOrDefault(e => e.ExpenseId == id);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST: Delete action - Handle the deletion of the expense
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Expense? expense = _db.expenses.Include(e => e.Category).Include(e => e.PaymentMethod).FirstOrDefault(e => e.ExpenseId == id);

            if (expense == null)
            {
                return NotFound();
            }

            // Remove the expense from the database
            _db.expenses.Remove(expense);
            _db.SaveChanges();

            // Optional: Display success message
            TempData["success"] = "Expense deleted successfully";

            return RedirectToAction("Index");
        }



        public IActionResult Index()
        {
            var expenses = _db.expenses
                .Include(e => e.Category)
                .Include(e => e.PaymentMethod)
                .ToList();
            return View(expenses);
        }

        //public IActionResult ExpensesByCategory()
        //{
        //    var groupedData = _db.expenses
        //        .GroupBy(e => e.Category)
        //        .Select(g => new
        //        {
        //            Category = g.Key,
        //            TotalAmount = g.Sum(e => e.Amount)
        //        }).ToList();

        //    return View(groupedData);
        //}

        public IActionResult ExpensesByCategory()
        {
            var groupedData = _db.expenses
                .GroupBy(e => e.Category)  // Assuming Category is a reference to a Category model
                .Select(g => new
                {
                    Category = g.Key,  // Category object (e.g., ExpenseTracker.Models.Category)
                    TotalAmount = g.Sum(e => e.Amount)
                }).ToList();

            return View(groupedData);  // Pass grouped data to the view
        }



        //public IActionResult ExpensesByDate()
        //{
        //    var groupedData = _db.expenses
        //        .GroupBy(e => e.Date)
        //        .Select(g => new
        //        {
        //            Date = g.Key,
        //            TotalAmount = g.Sum(e => e.Amount)
        //        }).ToList();

        //    return View(groupedData);

        //}
        public IActionResult ExpensesByDate()
        {
            var groupedData = _db.expenses
                .GroupBy(e => e.Date)  // Group by Date
                .Select(g => new
                {
                    Date = g.Key,  // Date as the key
                    TotalAmount = g.Sum(e => e.Amount)  // Total Amount for that date
                })
                .ToList();

            return View(groupedData);  // Return grouped data to the view
        }

        public IActionResult ExpensesByPaymentMethod()
        {
            var groupedData = _db.expenses
                .Include(e => e.PaymentMethod)  // Include PaymentMethod to access its properties
                .GroupBy(e => e.PaymentMethod)  // Group by PaymentMethod (which is a navigation property)
                .Select(g => new
                {
                    PaymentMethod = g.Key,  // PaymentMethod object
                    TotalAmount = g.Sum(e => e.Amount)  // Total amount for that payment method
                })
                .ToList();

            return View(groupedData);  // Return the grouped data to the view
        }

        //public class ExpenseOverviewModel
        //{
        //    public IEnumerable<dynamic> CategoryData { get; set; }
        //    public IEnumerable<dynamic> DateData { get; set; }
        //    public IEnumerable<dynamic> PaymentMethodData { get; set; }
        //}

        public IActionResult Overview()
        {
            // Fetch data
            var groupedDataByCategory = _db.expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key.Name,
                    TotalAmount = g.Sum(e => e.Amount)
                }).ToList();

            var groupedDataByDate = _db.expenses
                .GroupBy(e => e.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("MM/dd/yyyy"),
                    TotalAmount = g.Sum(e => e.Amount)
                }).ToList();

            var groupedDataByPaymentMethod = _db.expenses
                .Include(e => e.PaymentMethod)
                .GroupBy(e => e.PaymentMethod)
                .Select(g => new
                {
                    PaymentMethod = g.Key.MethodName,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .ToList();

            // Create view model
            var model = new ExpenseOverviewModel
            {
                CategoryData = groupedDataByCategory,
                DateData = groupedDataByDate,
                PaymentMethodData = groupedDataByPaymentMethod
            };

            return View(model);
        }

    }
}
