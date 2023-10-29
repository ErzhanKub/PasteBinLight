using Application.Shared;

namespace Infrastructure.DataBase
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task SaveCommitAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }

}