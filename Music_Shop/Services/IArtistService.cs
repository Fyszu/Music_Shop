using Music_Shop.Data;

namespace Music_Shop.Services
{
    public interface IArtistService : IService<Artist, int>
    {
        Task<Artist> GetByName(string name);
        Task<Artist> GetByAlbum(Album album);
    }
}
