using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    public class PosteRepository : IPosteRepository
    {
        public Task CreateAsync(Poste entity)
        {
            throw new NotImplementedException();
        }

        public Task<Guid[]> DeleteRangeAsync(params Guid[] id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Poste>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Poste> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Poste entity)
        {
            throw new NotImplementedException();
        }
    }
}