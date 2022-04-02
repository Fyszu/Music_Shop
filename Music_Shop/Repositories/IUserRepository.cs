using Music_Shop.Data;

namespace Music_Shop.Repositories
{
    public interface IUserRepository : IRepository<User, string>
    {
        public Task<User> GetByName(string name);
    }
}
