using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyDbContext _context;
        public OrderRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<Order> GetById(int id)
        {
            return await _context.Orders.Include(order => order.Products).Include(order => order.Buyer).Where(order => order.Id == id).SingleOrDefaultAsync();
        }

        public async Task Add(Order order)
        {
            if (order != null)
                await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Order order)
        {
            Order dbOrder = _context.Orders.Find(order.Id);
            if (dbOrder != null)
            {
                dbOrder.Id = order.Id;
                dbOrder.Currency = order.Currency;
                dbOrder.Buyer = order.Buyer;
                dbOrder.Products = order.Products;
                dbOrder.CustomerIp = order.CustomerIp;
                dbOrder.Description = order.Description;
                dbOrder.OrderDateTime = order.OrderDateTime;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Order order)
        {
            if (order != null)
                _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders.Include(order => order.Products).Include(order => order.Buyer).ToListAsync();
        }

        public async Task<List<Order>> GetByDate(DateTime date)
        {
            return await _context.Orders.Include(order => order.Products).Include(order => order.Buyer).Where(order => order.OrderDateTime.Date == date.Date).ToListAsync();
        }

        public async Task<List<Order>> GetBetweenTwoDates(DateTime from, DateTime to)
        {
            return await _context.Orders.Include(order => order.Products).Include(order => order.Buyer).Where(order => order.OrderDateTime.Date >= from.Date && order.OrderDateTime.Date <= to.Date).ToListAsync();
        }

        public async Task<List<Order>> GetByPrice(int price)
        {
            return await _context.Orders.Include(order => order.Products).Include(order => order.Buyer).Where(order => order.TotalPrice == price).ToListAsync();
        }

        public async Task<List<Order>> GetByBuyer(User buyer)
        {
            return await _context.Orders.Include(order => order.Products).Include(order => order.Buyer).Where(order => order.Buyer == buyer).ToListAsync();
        }
    }
}
