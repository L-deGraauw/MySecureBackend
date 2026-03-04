using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IEnvironment2DRepository
    {
        Task InsertAsync(Environment2D environment);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Environment2D>> SelectAsync();
        Task<Environment2D?> SelectAsync(Guid id);
        Task UpdateAsync(Environment2D environment);
        Task<int> CountByOwnerAsync(string ownerUserId);
        Task<bool> ExistsByOwnerAndNameAsync(string ownerUserId, string name);
    }
}
