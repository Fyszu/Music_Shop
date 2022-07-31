using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly MyDbContext _context;
        public TransactionRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<Order> GetById(int id)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Id == id).SingleOrDefaultAsync();
        }
        public async Task Add(Order transaction)
        {
            if (transaction != null)
                await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Order transaction)
        {
            Order dbTransaction = _context.Transactions.Find(transaction.Id);
            if (dbTransaction != null)
            {
                dbTransaction.Id = transaction.Id;
                dbTransaction.Buyer = transaction.Buyer;
                dbTransaction.Price = transaction.Price;
                dbTransaction.Album = transaction.Album;
                dbTransaction.DateTime = transaction.DateTime;
            }
            await _context.SaveChangesAsync();
        }
        public async Task Remove(Order transaction)
        {
            if (transaction != null)
                _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Order>> GetAll()
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).ToListAsync();
        }
        public async Task<List<Order>> GetByBuyer(User buyer)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Buyer.Equals(buyer)).ToListAsync();
        }
        public async Task<List<Order>> GetByAlbum(Album album)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Album.Equals(album)).ToListAsync();
        }
        public async Task<List<Order>> GetByDateTime(DateTime dateTime)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.DateTime.Equals(dateTime)).ToListAsync();
        }
        public async Task<List<Order>> GetByPrice(float price)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Price.Equals(price)).ToListAsync();
        }
    }
}
