using Music_Shop.Data;
using Music_Shop.Repositories;

namespace Music_Shop.Services
{
    public interface ICartService : IService<Cart,string>
    {
        Task<Cart> GetByUser(User user);
        Task<Cart> CreateNewCart(User? user);
        Task<Cart> AddItemToCart(Cart cart, CartItem item);
        Task<Cart> AddItemToCart(Cart cart, CartItem item, int quantity);
        Task<Cart> RemoveItemFromCart(Cart cart, CartItem item);
        Task<Cart> RemoveItemFromCart(Cart cart, CartItem item, int quantity);
        Task<string> GenerateGuid();
        Task<Cart> GetCartForGuest(HttpContext httpContext);
    }
}
