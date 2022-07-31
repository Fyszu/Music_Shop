﻿using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface ITransactionRepository : IRepository<Order, int>
    {
        Task<List<Order>> GetByBuyer(User buyer);
        Task<List<Order>> GetByAlbum(Album album);
        Task<List<Order>> GetByDateTime(DateTime dateTime);
        Task<List<Order>> GetByPrice(float price);
    }
}
