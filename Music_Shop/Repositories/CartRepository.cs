using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly MyDbContext _context;
        public CartRepository(MyDbContext context)
        {
            _context = context;
        }
        public async Task<Cart> GetByUser(User user)
        {
            return await _context.Carts.Include(cart => cart.User).Include(cart => cart.Items).Where(cart => cart.User == user).SingleOrDefaultAsync();
        }

        public async Task Add(Cart entity)
        {
            if (entity == null) return;
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Cart>> GetAll()
        {
            return await _context.Carts.Include(cart => cart.User).Include(cart => cart.Items).ToListAsync();
        }

        public async Task<Cart> GetById(string id)
        {
            return await _context.Carts.Include(cart => cart.User).Include(cart => cart.Items).Where(cart => cart.Id == id).SingleOrDefaultAsync();
        }

        public async Task Remove(Cart entity)
        {
            if (entity == null) return;
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Cart entity)
        {
            Cart dbCart = _context.Carts.Find(entity.Id); 
            if (dbCart != null)
            {
                dbCart.Id = entity.Id;
                dbCart.User = entity.User;
                dbCart.Items = entity.Items;
            }
            await _context.SaveChangesAsync();
        }
    }
}
