using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Music_Shop.Data
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CustomerIp { get; set; }
        [Required]
        public string Description { get; set; }
        public string Currency { get; set; } = "PLN";
        [ForeignKey("UserId")]
        public User Buyer { get; set; }
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public DateTime OrderDateTime { get; set; }
        public int GetTotalPrice()
        {
            int result = 0;
            foreach(Product product in Products)
            {
                result += product.TotalPrice;
            }
            return result;
        }
    }
}
