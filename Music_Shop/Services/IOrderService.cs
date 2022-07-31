using Music_Shop.Data;

namespace Music_Shop.Services
{
    public interface IOrderService : IService<Order, int>
    {
        Task<List<Order>> GetByBuyer(User buyer);
        Task<List<Order>> GetByAlbum(Album album);
        Task<List<Order>> GetByDateTime(DateTime dateTime);
        Task<List<Order>> GetByPrice(float price);
        public InternalResponse GetBearerToken();
        public InternalResponse OrderRequest(Order order, string bearerAuth);
    }
}
