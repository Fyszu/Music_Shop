using Music_Shop.Data;
using Music_Shop.Repositories;

namespace Music_Shop.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserRepository _repository;

        public UserService(ILogger<UserService> logger, UserRepository userRepository)
        {
            _logger = logger;
            _repository = userRepository;
        }


        public async Task<User> GetById(string id) { return await _repository.GetById(id); }
        public async Task Add(User user) { await _repository.Add(user); }
        public async Task Update(User user) { await _repository.Update(user); }
        public async Task Remove(User user) { await _repository.Remove(user); }
        public async Task<List<User>> GetAll() { return await _repository.GetAll(); }
        public async Task<User> GetByName(string name) { return await _repository.GetByName(name); }
    }
}
