using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface IArtistRepository : IRepository<Artist, int>
    {
        Task<Artist> GetByName(string name);
        Task<Artist> GetByAlbum(Album album);
    }
}
