using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction, int>
    {
        Task<List<Transaction>> GetByBuyer(User buyer);
        Task<List<Transaction>> GetByAlbum(Album album);
        Task<List<Transaction>> GetByDateTime(DateTime dateTime);
        Task<List<Transaction>> GetByPrice(float price);
    }
}
