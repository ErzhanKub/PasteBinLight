using Domain.Entities;
using Domain.Repositories;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RecordRepository : IRecordRepository
{
    private readonly AppDbContext _dbcontext;

    public RecordRepository(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    public async Task<Guid> CreateAsync(Record entity)
    {
        await _dbcontext.Postes.AddAsync(entity);
        return entity.Id;
    }

    public Task<Guid[]> DeleteByIdAsync(params Guid[] ids)
    {
        var posteToDelete = _dbcontext.Postes.Where(p => ids.Contains(p.Id));
        _dbcontext.Postes.RemoveRange(posteToDelete);
        return Task.FromResult(ids);
    }

    public async Task<Record?> GetByIdAsync(Guid id)
    {
        var poste = await _dbcontext.Postes.FirstOrDefaultAsync(p => p.Id == id);
        return poste;
    }

    public Guid GetDecodedGuid(string decodedBytes)
    {
        byte[] decoded = Convert.FromBase64String(decodedBytes);
        var guid = new Guid(decoded);
        return guid;
    }

    public string GetEncodedGuid(Guid guid)
    {
        byte[] guidBytes = guid.ToByteArray();
        string base64Guid = Convert.ToBase64String(guidBytes);
        return base64Guid;
    }

    public void Update(Record entity)
    {
        _dbcontext.Postes.Update(entity);
    }

    public async Task<IReadOnlyList<Record>> GetAllAsync()
    {
        return await _dbcontext.Postes.AsNoTracking().ToListAsync();
    }
}