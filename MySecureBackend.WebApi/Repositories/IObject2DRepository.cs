using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IObject2DRepository
    {
        Task InsertAsync(Object2D object2D);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Object2D>> SelectAsync();
        Task<Object2D?> SelectAsync(Guid id);
        Task<IEnumerable<Object2D>> SelectByEnvironmentAsync(Guid environmentId);
        Task UpdateAsync(Object2D object2D);
    }
}
