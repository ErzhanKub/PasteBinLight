namespace Domain.Shared
{
    // Generic repository interface
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // Fetch an entity by its ID
        Task<TEntity?> FetchByIdAsync(Guid id, CancellationToken cancellationToken);

        // Fetch all entities with pagination
        Task<IReadOnlyList<TEntity>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        // Create a new entity
        Task<Guid> CreateAsync(TEntity entity, CancellationToken cancellationToken);

        // Update an existing entity
        void Update(TEntity entity);

        // Remove entities by their IDs
        Task<Guid[]> RemoveByIdAsync(params Guid[] id);
    }
}
