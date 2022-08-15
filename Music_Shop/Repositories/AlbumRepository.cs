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
            return await _context.Albums.Include(album => album.Artists).Include(a => a.Orders).Where(album => album.Id == id).SingleOrDefaultAsync();
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
                dbAlbum.Artists = album.Artists;
                dbAlbum.Name = album.Name;
                dbAlbum.Description = album.Description;
                dbAlbum.Price = album.Price;
                dbAlbum.Orders = album.Orders;
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
            return await _context.Albums.Include(album => album.Artists).Include(a => a.Orders).ToListAsync();
        }

        public async Task<Album> GetByName(string name)
        {
            return await _context.Albums.Include(album => album.Artists).Include(a => a.Orders).Where(a => a.Name == name).SingleOrDefaultAsync();
        }

        public async Task<List<Album>> GetByArtists(HashSet<Artist> artists)
        {
            List<Album> albums = new();
            foreach (Artist artist in artists)
                albums.Add(await _context.Albums.Include(album => album.Artists).Include(a => a.Orders).Where(album => album.Artists.Contains(artist)).FirstOrDefaultAsync());
            return albums;
        }

        public async Task<List<Album>> GetByPrice(float price)
        {
            return await _context.Albums.Include(album => album.Artists).Include(a => a.Orders).Where(album => album.Price == price).ToListAsync();
        }
    }
}
