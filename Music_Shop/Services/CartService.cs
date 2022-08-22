using Music_Shop.Data;
using Music_Shop.Repositories;
using Org.BouncyCastle.Utilities;

namespace Music_Shop.Services
{
    public class CartService : ICartService
    {
        private readonly CartRepository _repository;
        private const string CartSessionKey = "CartId";
        public CartService(ILogger<ArtistService> logger, CartRepository cartRepository)
        {
            _repository = cartRepository;
        }
        public async Task Add(Cart entity)
        {
            await _repository.Add(entity);
        }

        public async Task<List<Cart>> GetAll()
        {
            return await _repository.GetAll();
        }

        public async Task<Cart> GetById(string id)
        {
            return await _repository.GetById(id);
        }

        public async Task<Cart> GetByUser(User user)
        {
            return await _repository.GetByUser(user); 
        }

        public async Task Remove(Cart entity)
        {
            await _repository.Remove(entity);
        }

        public async Task Update(Cart entity)
        {
            await _repository.Update(entity);
        }
        public async Task<Cart> CreateNewCart(User? user)
        {
            Cart cart = new(await GenerateGuid(), user);
            await _repository.Add(cart);
            return cart;
        }
        public async Task<Cart> AddItemToCart(Cart cart, CartItem item)
        {
            CartItem? oldItem = cart.Items.Where(i => i.Id == item.Id).FirstOrDefault();
            if (oldItem == null)
            {
                item.Cart = cart;
                item.Quantity = 1;
                item.DateCreated = DateTime.Now;
                cart.Items.Add(item);
            }
            else
                oldItem.Quantity += 1;
            await _repository.Update(cart);
            return cart;
        }
        public async Task<Cart> AddItemToCart(Cart cart, CartItem item, int quantity)
        {
            CartItem? oldItem = cart.Items.Where(i => i.Id == item.Id).FirstOrDefault();
            if (oldItem == null)
            {
                item.Cart = cart;
                item.Quantity = quantity;
                item.DateCreated = DateTime.Now;
                cart.Items.Add(item);
            }
            else
                oldItem.Quantity += quantity;
            await _repository.Update(cart);
            return cart;
        }
        public async Task<Cart> RemoveItemFromCart(Cart cart, CartItem item)
        {
            CartItem? oldItem = cart.Items.Where(i => i.Id == item.Id).FirstOrDefault();
            if (oldItem == null)
                throw new Exception("Nie znaleziono przedmiotu w koszyku, który miał być usunięty.");
            else
                oldItem.Quantity -= 1;
            if (oldItem.Quantity <= 0)
                cart.Items.Remove(oldItem);
            await _repository.Update(cart);
            return cart;
        }
        public async Task<Cart> RemoveItemFromCart(Cart cart, CartItem item, int quantity)
        {
            CartItem? oldItem = cart.Items.Where(i => i.Id == item.Id).FirstOrDefault();
            if (oldItem == null)
                throw new Exception("Nie znaleziono przedmiotu w koszyku, który miał być usunięty.");
            else
                oldItem.Quantity -= quantity;
            if (oldItem.Quantity <= 0)
                cart.Items.Remove(oldItem);
            await _repository.Update(cart);
            return cart;
        }
        public async Task<string> GenerateGuid()
        {
            do
            {
                string id = Guid.NewGuid().ToString();
                if (await _repository.GetById(id) == null)
                    return id;
            }
            while (true);
        }
        public async Task<Cart> GetCartForGuest(HttpContext httpContext)
        {
            var session = httpContext.Session;
            string? cartId = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartId))
            {
                Cart cart = await CreateNewCart(null);
                session.SetString(CartSessionKey, cart.Id);
                return cart;
            }
            else
            {
                Cart? cart = await _repository.GetById(cartId);
                if (cart == null)
                {
                    cart = await CreateNewCart(null);
                    session.SetString(CartSessionKey, cart.Id);
                }
                return cart;
            }
        }
    }
}
