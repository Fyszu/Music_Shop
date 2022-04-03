using Music_Shop.Data;
using Music_Shop.Repositories;
using System.Text;
using XSystem.Security.Cryptography;

namespace Music_Shop.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ILogger<TransactionService> _logger;
        private readonly TransactionRepository _repository;

        public TransactionService(ILogger<TransactionService> logger, TransactionRepository transactionRepository)
        {
            _logger = logger;
            _repository = transactionRepository;
        }


        public async Task<Transaction> GetById(int id) { return await _repository.GetById(id); }
        public async Task Add(Transaction transaction) { await _repository.Add(transaction); }
        public async Task Update(Transaction transaction) { await _repository.Update(transaction); }
        public async Task Remove(Transaction transaction) { await _repository.Remove(transaction); }
        public async Task<List<Transaction>> GetAll() { return await _repository.GetAll(); }
        public async Task<List<Transaction>> GetByBuyer(User buyer) { return await _repository.GetByBuyer(buyer); }
        public async Task<List<Transaction>> GetByAlbum(Album album) { return await _repository.GetByAlbum(album); }
        public async Task<List<Transaction>> GetByDateTime(DateTime dateTime) { return await _repository.GetByDateTime(dateTime); }
        public async Task<List<Transaction>> GetByPrice(float price) { return await _repository.GetByPrice(price); }

        public string Generatetxnid()
        {
            Random random = new Random();
            string hash = Generatehash512(random.ToString() + DateTime.Now);
            string txnid = hash.ToString().Substring(0, 20);
            return txnid;
        }
        private string Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
    }
}
