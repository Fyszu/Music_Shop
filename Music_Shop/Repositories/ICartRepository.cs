using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface ICartRepository : IRepository<Cart, string>
    {
        Task<Cart> GetByUser(User user);
    }
}
