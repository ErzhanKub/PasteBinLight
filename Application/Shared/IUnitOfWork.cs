// Namespace for shared application features
namespace Application.Shared
{
    // Interface for the Unit of Work pattern
    public interface IUnitOfWork
    {
        // Method to save changes and commit the transaction asynchronously
        Task SaveAndCommitAsync(CancellationToken cancellationToken);
    }
}
