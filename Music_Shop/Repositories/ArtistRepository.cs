using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly MyDbContext _context;
        public ArtistRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<Artist> GetById(int id)
        {
            return await _context.Artists.Include(aritst => aritst.Albums).Where(artist => artist.Id == id).SingleOrDefaultAsync();
        }

        public async Task Add(Artist artist)
        {
            if (artist != null)
                await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Artist artist)
        {
            Artist dbArtist = _context.Artists.Find(artist.Id);
            if (dbArtist != null)
            {
                dbArtist.Id = artist.Id;
                dbArtist.Name = artist.Name;
                dbArtist.Albums = artist.Albums;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Remove(Artist artist)
        {
            if (artist != null)
                _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Artist>> GetAll()
        {
            return await _context.Artists.Include(artist => artist.Albums).ToListAsync();
        }

        public async Task<Artist> GetByName(string name)
        {
            return await _context.Artists.Include(aritst => aritst.Albums).Where(a => a.Name == name).SingleOrDefaultAsync();
        }

        public async Task<Artist> GetByAlbum(Album album)
        {
            return await _context.Artists.Include(artist => artist.Albums).Where(artist => artist.Albums.Contains(album)).SingleOrDefaultAsync();
        }
    }
}
