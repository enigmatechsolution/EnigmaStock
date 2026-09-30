using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EnigmaStock.Models
{
    public class StockMovement
    {
        public int Id { get; set; } 
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }

       
        public string MovementType {  get; set; } = string.Empty;

        public DateTime Date {  get; set; } = DateTime.Now;

        public string? Note { get; set; }   
    }
}
