namespace EnigmaStock.Models.viewModels
{
    public class RecentSaleViewModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime SaleDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
