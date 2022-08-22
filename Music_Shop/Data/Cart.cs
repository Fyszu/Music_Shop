using Music_Shop.Services;
using System.ComponentModel.DataAnnotations;

namespace Music_Shop.Data
{
    public class Cart
    {
        [Key]
        public string Id { get; set; }
        public Cart(string id, User? user)
        {
            Id = id;
            User = user;
        }
        public ICollection<CartItem> Items { get; set; } = new HashSet<CartItem>();
        public User? User { get; set; }
        public int TotalPrice
        {
            get
            {
                int totalPrice = 0;
                foreach (CartItem item in Items)
                    totalPrice += item.TotalPrice;
                return totalPrice;
            }
        }
    }
}
