using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly MyDbContext _context;
        public AlbumRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<Album> GetById(int id)
        {
            return await _context.Albums.Include(album => album.Owner).Include(a => a.Transactions).Where(album => album.Id == id).SingleOrDefaultAsync();
        }

        public async Task Add(Album album)
        {
            if (album != null)
                await _context.Albums.AddAsync(album);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Album album)
        {
            Album dbAlbum = _context.Albums.Find(album.Id);
            if (dbAlbum != null)
            {
                dbAlbum.Id = album.Id;
                dbAlbum.Owner = album.Owner;
                dbAlbum.Name = album.Name;
                dbAlbum.Description = album.Description;
                dbAlbum.Price = album.Price;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Album album)
        {
            if (album != null)
                _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Album>> GetAll()
        {
            return await _context.Albums.Include(album => album.Owner).Include(a => a.Transactions).ToListAsync();
        }

        public async Task<Album> GetByName(string name)
        {
            return await _context.Albums.Include(album => album.Owner).Include(a => a.Transactions).Where(a => a.Name == name).SingleOrDefaultAsync();
        }

        public async Task<List<Album>> GetByOwner(Artist owner)
        {
            return await _context.Albums.Include(album => album.Owner).Include(a => a.Transactions).Where(album => album.Owner.Equals(owner)).ToListAsync();
        }

        public async Task<List<Album>> GetByPrice(float price)
        {
            return await _context.Albums.Include(album => album.Owner).Include(a => a.Transactions).Where(album => album.Price == price).ToListAsync();
        }
    }
}
