namespace Domain.Shared
{
    public interface IRepository<TEntity>
    {
        Task<List<TEntity>> GetAllAsync();
        Task<Guid> CreateAsync(TEntity entity);
        void Update(TEntity entity);
        Task<Guid[]> DeleteRangeAsync(params Guid[] id);
        Task<TEntity> GetByIdAsync(Guid id);
    }
}