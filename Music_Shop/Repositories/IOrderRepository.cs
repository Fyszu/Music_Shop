using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface IOrderRepository : IRepository<Order, int>
    {
        Task<List<Order>> GetByDate(DateTime date);
        Task<List<Order>> GetBetweenTwoDates(DateTime from, DateTime to);
        Task<List<Order>> GetByPrice(int price);
        Task<List<Order>> GetByBuyer(User buyer);
    }
}