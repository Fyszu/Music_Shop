using Music_Shop.Data;

namespace Music_Shop.Services
{
    public interface IUserService : IService<User, string>
    {
        public Task<User> GetByName(string name);
    }
}
