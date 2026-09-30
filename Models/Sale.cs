using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EnigmaStock.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

       
        public string? InvoiceNumber { get; set; }


        [Required]
        public DateTime SaleDate { get; set; } = DateTime.Now;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";

        public ICollection<SaleItem> saleItems { get; set; } = new List<SaleItem>();
    }
}
