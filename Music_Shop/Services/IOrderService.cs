using Music_Shop.Data;

namespace Music_Shop.Services
{
    public interface IOrderService : IService<Order, int>
    {
        Task<List<Order>> GetByBuyer(User buyer);
        Task<List<Order>> GetByDate(DateTime dateTime);
        Task<List<Order>> GetBetweenTwoDates(DateTime from, DateTime to);
        Task<List<Order>> GetByPrice(int price);
        public string GetPaymentRedirectionUrl(Order order);
    }
}
