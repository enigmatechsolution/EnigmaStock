using Microsoft.AspNetCore.Mvc;
using EnigmaStock.Data;
using EnigmaStock.Models.viewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace EnigmaStock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }



        public IActionResult Index()
        {

            var completedSales = _db.Sales
                .Where(s => s.Status == "Completed")
                .Include(s => s.saleItems)
                .ToList();

            var totalRevenue = completedSales.
                Sum(s => s.saleItems.Sum(item => item.Quantity * item.SellingPrice));

            var completedSaleCount = completedSales.Count;

            var averageSaleValue = completedSaleCount > 0
                ? totalRevenue / completedSaleCount
                : 0;

            var sixMonthsAgo = DateTime.Now.AddMonths(-5);



            var startMonth = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
        1
    ).AddMonths(-5);

            var salesTrend = Enumerable.Range(0, 6)
                .Select(i =>
                {
                    var month = startMonth.AddMonths(i);

                    var revenue = completedSales
                        .Where(s =>
                            s.SaleDate.Year == month.Year &&
                            s.SaleDate.Month == month.Month)
                        .Sum(s =>
                            s.saleItems.Sum(item =>
                                item.Quantity * item.SellingPrice));

                    return new SalesTrendViewModel
                    {
                        Month = month.ToString("MMM yyyy"),
                        Revenue = revenue
                    };
                })
                .ToList();


            var recentSales = _db.Sales
                .Include(s => s.saleItems)
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .Select(s => new RecentSaleViewModel

                {
                    Id = s.Id,
                    InvoiceNumber = s.InvoiceNumber,
                    CustomerName = s.CustomerName,
                    SaleDate = s.SaleDate,
                    PaymentMethod = s.PaymentMethod,
                    Status = s.Status,
                    TotalAmount = s.saleItems.Sum(item => item.Quantity * item.SellingPrice)
                })
                .ToList();
            
           var model = new DashboardViewModel
           {
               CategoryCount = _db.Categories.Count(),
               ProductCount = _db.Products.Count(),
               SupplierCount = _db.Suppliers.Count(),
               PurchaseCount = _db.Purchases.Count(),

               SaleCount = _db.Sales.Count(),
               CompletedSaleCount = completedSaleCount,

               TotalRevenue = totalRevenue,AverageSaleValue = averageSaleValue,

               LowStockCount = _db.Products.Count(p => p.Quantity <= p.ReorderLevel),
               LowStockProducts = _db.Products.Where(p => p.Quantity <= p.ReorderLevel).ToList(),

               RecentSales = recentSales,
               SalesTrend =salesTrend
           };

            return View(model);
        }
    }
}
