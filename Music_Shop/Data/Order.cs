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
        public string Description { get { return $"Zamówienie nr. {this.Id}"; } }
        public string AdditionalDescription {
            get {
                string additionalDescription = string.Empty;
                int productNumber = 1;
                foreach(CartItem product in Products)
                    additionalDescription += $"Produkt nr. {productNumber}: ID: {product.Id}, {product.Name} ({product.Description}), ilość: {product.Quantity}.   ";
                return additionalDescription;
            }
        }
        public string Currency { get; set; } = "PLN";
        [ForeignKey("UserId")]
        public User Buyer { get; set; }
        public ICollection<CartItem> Products { get; set; } = new HashSet<CartItem>();
        public DateTime OrderDateTime { get; set; }
        public int TotalPrice { get {
                int result = 0;
                foreach (CartItem product in Products)
                {
                    result += product.TotalPrice;
                }
                return result;
            }
        }
    }
}
