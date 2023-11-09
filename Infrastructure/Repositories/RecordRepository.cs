using Domain.Entities;
using Domain.Repositories;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

// Repository class for Record entity
public sealed class RecordRepository : IRecordRepository
{
    // Database context
    private readonly AppDbContext _dbcontext;

    // Constructor
    public RecordRepository(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    // Create a new record
    public async Task<Guid> CreateAsync(Record entity, CancellationToken cancellationToken)
    {
        await _dbcontext.Records.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    // Delete records by their IDs
    public Task<Guid[]> RemoveByIdAsync(params Guid[] ids)
    {
        var posteToDelete = _dbcontext.Records.Where(p => ids.Contains(p.Id));
        _dbcontext.Records.RemoveRange(posteToDelete);
        return Task.FromResult(ids);
    }

    // Get a record by its ID
    public async Task<Record?> FetchByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var poste = await _dbcontext.Records.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return poste;
    }

    // Decode a GUID from a base64 string
    public Guid DecodeGuidFromBase64(string base64Guid)
    {
        byte[] decoded = Convert.FromBase64String(base64Guid);
        var guid = new Guid(decoded);
        return guid;
    }

    // Encode a GUID to a base64 string
    public string EncodeGuidToBase64(Guid guid)
    {
        byte[] guidBytes = guid.ToByteArray();
        string base64Guid = Convert.ToBase64String(guidBytes);
        return base64Guid;
    }

    // Update a record
    public void Update(Record entity)
    {
        _dbcontext.Records.Update(entity);
    }

    // Get all records
    public async Task<IReadOnlyList<Record>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _dbcontext.Records
        .AsNoTracking()
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
    }

    // Search records by title
    public async Task<IReadOnlyList<Record>> FindRecordsByTitleAsync(string title, CancellationToken cancellationToken)
    {
        var records = await _dbcontext.Records.Where(r => r.Title.Contains(title) && r.IsPrivate == false).ToListAsync(cancellationToken) ?? new List<Record>();
        return records;
    }

    // Get top 100 posts by number of likes
    public async Task<IReadOnlyList<Record>> FetchTop100RecordsByLikesAsync(CancellationToken cancellationToken)
    {
        return await _dbcontext.Records
            .Where(r => r.IsPrivate == false)
            .AsNoTracking()
            .OrderByDescending(r => r.Likes)
            .Take(100)
            .ToListAsync(cancellationToken);
    }
}