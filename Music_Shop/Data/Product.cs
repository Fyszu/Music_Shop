using System.ComponentModel.DataAnnotations;

namespace Music_Shop.Data
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Price { get; set; } // Podawany w najmniejszych jednostkach, np. grosz
        public int Quantity { get; set; } = 0;
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
        public int TotalPrice { get { return Price * Quantity; } }
    }
}
