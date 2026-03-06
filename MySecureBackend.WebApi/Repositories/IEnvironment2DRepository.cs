using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IEnvironment2DRepository
    {
        Task<Guid> InsertAsync(Environment2D environment);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Environment2D>> SelectAsync();
        Task<IEnumerable<Environment2D>> SelectByOwnerAsync(string ownerUserId);
        Task<Environment2D?> SelectAsync(Guid id);
        Task UpdateAsync(Environment2D environment);
        Task<int> CountByOwnerAsync(string ownerUserId);
        Task<bool> ExistsByOwnerAndNameAsync(string ownerUserId, string name);
    }
}
