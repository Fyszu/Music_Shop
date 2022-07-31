using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Music_Shop.Data
{
    public class User : IdentityUser
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Language { get; set; } = "pl";
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
