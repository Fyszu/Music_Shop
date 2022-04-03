using Music_Shop.Data;

namespace Music_Shop.Services
{
    public interface ITransactionService : IService<Transaction, int>
    {
        Task<List<Transaction>> GetByBuyer(User buyer);
        Task<List<Transaction>> GetByAlbum(Album album);
        Task<List<Transaction>> GetByDateTime(DateTime dateTime);
        Task<List<Transaction>> GetByPrice(float price);
        public string Generatetxnid();
        public string Generatehash512(string text);

    }
}
