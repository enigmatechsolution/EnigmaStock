using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnigmaStock.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        [Required] 
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")] 
        public Supplier? Supplier { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        [Required] 
        public string ReferenceNumber { get; set; } = string.Empty;
        public ICollection<PurchaseItem> purchaseItems { get; set; } = new List<PurchaseItem>();
    }
}
 