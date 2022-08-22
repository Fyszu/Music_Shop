using Music_Shop.Data;
using Music_Shop.Repositories;

namespace Music_Shop.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ILogger<ArtistService> _logger;
        private readonly ArtistRepository _repository;

        public ArtistService(ILogger<ArtistService> logger, ArtistRepository artistRepository)
        {
            _logger = logger;
            _repository = artistRepository;
        }


        public async Task<Artist> GetById(int id) { return await _repository.GetById(id); }
        public async Task Add(Artist artist) { await _repository.Add(artist); }
        public async Task Update(Artist artist) { await _repository.Update(artist); }
        public async Task Remove(Artist artist) { await _repository.Remove(artist); }
        public async Task<List<Artist>> GetAll() { return await _repository.GetAll(); }
        public async Task<Artist> GetByName(string name) { return await _repository.GetByName(name); }
        public async Task<Artist> GetByAlbum(Album album) { return await _repository.GetByAlbum(album); }
    }
}
