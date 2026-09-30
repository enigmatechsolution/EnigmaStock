using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaStock.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        [Required] public int PurchaseId { get; set; }
        [ForeignKey("PurchaseId")]  public Purchase? purchase { get; set; }

        [Required] public int ProductId { get; set; }
        [ForeignKey("ProductId")] public Product? product { get; set; }
        [Required] public int Quantity { get; set; }
        [Required] public double CostPrice { get; set; }
    }
}
