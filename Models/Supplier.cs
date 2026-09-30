using System.ComponentModel.DataAnnotations;
namespace EnigmaStock.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
