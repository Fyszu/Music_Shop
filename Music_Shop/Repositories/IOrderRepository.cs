using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetById(int id);
        Task Add(Order order);

        Task Update(Order order);

        Task Remove(Order order);

        Task<List<Order>> GetAll();

        Task<List<Order>> GetByDate(DateTime date);

        Task<List<Order>> GetBetweenTwoDates(DateTime from, DateTime to);

        Task<List<Order>> GetByPrice(int price);
        Task<List<Order>> GetByBuyer(User buyer);
    }
}