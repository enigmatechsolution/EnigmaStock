using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EnigmaStock.Models
{
    public class SaleItem
    {
        [Key] public int Id { get; set; }

        public int SaleId { get; set; }

        [ForeignKey("SaleId")]
        public Sale? Sale { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required] public int Quantity { get; set; }

        [Required] public decimal SellingPrice { get; set; } 
    }
}
