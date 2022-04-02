namespace Music_Shop.Services
{
    public interface IService<TEntity, TId>
    {
        Task<TEntity> GetById(TId id);
        Task Add(TEntity entity);
        Task Update(TEntity entity);
        Task Remove(TEntity entity);
        Task<List<TEntity>> GetAll();
    }
}
