using System.ComponentModel.DataAnnotations;

namespace Music_Shop.Data
{
    public class CartItem : Product
    {
        [Required]
        public Cart Cart { get; set; }
        [Required]
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public int TotalPrice { get { return Price * Quantity; } }
    }
}
