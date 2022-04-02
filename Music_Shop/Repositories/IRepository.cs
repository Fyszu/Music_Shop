namespace Music_Shop.Repositories
{
    public interface IRepository<TEntity, TId>
    {
        Task<TEntity> GetById(TId id);
        Task Add(TEntity entity);
        Task Update(TEntity entity);
        Task Remove(TEntity entity);
        Task<List<TEntity>> GetAll();
    }
}
