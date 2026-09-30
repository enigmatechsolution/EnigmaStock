using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EnigmaStock.Models
{
    public class Product
    {
      
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string SKU { get; set; }

        [Required]
        public decimal CostPrice { get; set; }

        [Required]
        public decimal SellingPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int ReorderLevel { get; set; }


        public string Description { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
