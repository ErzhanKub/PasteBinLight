namespace Domain.Shared;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<Guid> CreateAsync(TEntity entity);
    void Update(TEntity entity);
    Task<Guid[]> DeleteRangeAsync(params Guid[] id);
}