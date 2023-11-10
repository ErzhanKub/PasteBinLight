using Application.Shared;

namespace Infrastructure.DataBase;

// UnitOfWork class that implements the IUnitOfWork interface
public class UnitOfWork : IUnitOfWork
{
    // Database context
    private readonly AppDbContext _dbContext;

    // Constructor
    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Save changes and commit the transaction asynchronously
    public async Task SaveAndCommitAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync();
    }
}
