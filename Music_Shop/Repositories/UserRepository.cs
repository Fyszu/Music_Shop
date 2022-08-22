using Microsoft.EntityFrameworkCore;
using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _context;
        public UserRepository(MyDbContext context)
        {
            _context = context;
        }


        public async Task<User> GetById(string id)
        {
            return await _context.Users.Include(u => u.Orders).Include(u => u.Cart).Where(u => u.Id.Equals(id)).SingleOrDefaultAsync();
        }

        public async Task Add(User user)
        {
            if (user != null)
                await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            User dbUser = _context.Users.Find(user.Id);
            if (dbUser != null)
            {
                dbUser.Email = user.Email;
                dbUser.UserName = user.UserName;
                dbUser.PasswordHash = user.PasswordHash;
                dbUser.Id = user.Id;
                dbUser.Street = user.Street;
                dbUser.City = user.City;
                dbUser.PostalCode = user.PostalCode;
            }
            await _context.SaveChangesAsync();
        }

        public async Task Remove(User user)
        {
            if (user != null)
                _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.Include(u => u.Orders).Include(u => u.Cart).ToListAsync();
        }

        public async Task<User> GetByName(string name)
        {
            return await _context.Users.Include(u => u.Orders).Include(u => u.Cart).Where(u => u.UserName == name).SingleOrDefaultAsync();
        }
    }
}
