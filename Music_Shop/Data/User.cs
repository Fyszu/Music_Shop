using Microsoft.AspNetCore.Identity;

namespace Music_Shop.Data
{
    public class User : IdentityUser
    {
        public ICollection<Transaction> Transactions { get; set; }

        public User()
        {
            Transactions = new HashSet<Transaction>();
        }
    }
}
