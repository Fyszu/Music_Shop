using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface IAlbumRepository : IRepository<Album, int>
    {
        Task<Album> GetByName(string name);
        Task<List<Album>> GetByOwner(Artist owner);
        Task<List<Album>> GetByPrice(float price);
    }
}
