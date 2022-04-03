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


        public async Task<Transaction> GetById(int id)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Id == id).SingleOrDefaultAsync();
        }
        public async Task Add(Transaction transaction)
        {
            if (transaction != null)
                await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Transaction transaction)
        {
            Transaction dbTransaction = _context.Transactions.Find(transaction.Id);
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
        public async Task Remove(Transaction transaction)
        {
            if (transaction != null)
                _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Transaction>> GetAll()
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).ToListAsync();
        }
        public async Task<List<Transaction>> GetByBuyer(User buyer)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Buyer.Equals(buyer)).ToListAsync();
        }
        public async Task<List<Transaction>> GetByAlbum(Album album)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Album.Equals(album)).ToListAsync();
        }
        public async Task<List<Transaction>> GetByDateTime(DateTime dateTime)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.DateTime.Equals(dateTime)).ToListAsync();
        }
        public async Task<List<Transaction>> GetByPrice(float price)
        {
            return await _context.Transactions.Include(transaction => transaction.Buyer).Include(transaction => transaction.Album).Where(t => t.Price.Equals(price)).ToListAsync();
        }
    }
}
