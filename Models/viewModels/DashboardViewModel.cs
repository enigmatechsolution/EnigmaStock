namespace EnigmaStock.Models.viewModels
{
    public class DashboardViewModel
    {
        public int CategoryCount { get; set; }
        public int ProductCount { get; set; }
        public int SupplierCount { get; set; }
        public int PurchaseCount { get; set; }
        public int SaleCount { get; set; }
        public int CompletedSaleCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageSaleValue { get; set; }
        public int LowStockCount { get; set; }
       
        public List<Product> LowStockProducts { get; set; } = new List<Product>();
        public List<RecentSaleViewModel> RecentSales { get; set; } = new List<RecentSaleViewModel>();

        public List<SalesTrendViewModel> SalesTrend { get; set; } = new List<SalesTrendViewModel>();
    }
}
