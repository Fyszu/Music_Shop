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
        public int StockQuantity { get; set; }
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
        public ICollection<CartItem> Items { get; set; } = new HashSet<CartItem>();
    }
}
