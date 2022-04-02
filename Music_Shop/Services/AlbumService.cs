using Music_Shop.Data;
using Music_Shop.Repositories;

namespace Music_Shop.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly ILogger<AlbumService> _logger;
        private readonly AlbumRepository _repository;

        public AlbumService(ILogger<AlbumService> logger, AlbumRepository albumRepository)
        {
            _logger = logger;
            _repository = albumRepository;
        }


        public async Task<Album> GetById(int id) { return await _repository.GetById(id); }
        public async Task Add(Album album) { await _repository.Add(album); }
        public async Task Update(Album album) { await _repository.Update(album); }
        public async Task Remove(Album album) { await _repository.Remove(album); }
        public async Task<List<Album>> GetAll() { return await _repository.GetAll(); }
        public async Task<Album> GetByName(string name) { return await _repository.GetByName(name); }
        public async Task<List<Album>> GetByOwner(Artist owner) { return await _repository.GetByOwner(owner); }
        public async Task<List<Album>> GetByPrice(float price) { return await _repository.GetByPrice(price); }
    }
}
