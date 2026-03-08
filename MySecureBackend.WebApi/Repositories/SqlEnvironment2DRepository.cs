using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlEnvironment2DRepository : IEnvironment2DRepository
    {
        private readonly string sqlConnectionString;

        public SqlEnvironment2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<Guid> InsertAsync(Environment2D environment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleAsync<Guid>("INSERT INTO [Environment2D] (Name, OwnerUserId, MaxLength, MaxHeight) OUTPUT INSERTED.Id VALUES (@Name, @OwnerUserId, @MaxLength, @MaxHeight)", environment);
            }
        }

        public async Task<Environment2D?> SelectAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Environment2D>("SELECT Id, Name, OwnerUserId, MaxLength, MaxHeight FROM [Environment2D] WHERE Id = @id", new { id });
            }
        }

        public async Task<IEnumerable<Environment2D>> SelectAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Environment2D>("SELECT Id, Name, OwnerUserId, MaxLength, MaxHeight FROM [Environment2D]");
            }
        }

        public async Task<IEnumerable<Environment2D>> SelectByOwnerAsync(string ownerUserId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Environment2D>("SELECT Id, Name, OwnerUserId, MaxLength, MaxHeight FROM [Environment2D] WHERE OwnerUserId = @ownerUserId", new { ownerUserId });
            }
        }

        public async Task UpdateAsync(Environment2D environment)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("UPDATE [Environment2D] SET " +
                                                 "Name = @Name, " +
                                                 "OwnerUserId = @OwnerUserId, " +
                                                 "MaxLength = @MaxLength, " +
                                                 "MaxHeight = @MaxHeight " +
                                                 "WHERE Id = @Id", environment);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync("DELETE FROM [Environment2D] WHERE Id = @Id", new { id });
            }
        }

        public async Task<int> CountByOwnerAsync(string ownerUserId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM [Environment2D] WHERE OwnerUserId = @ownerUserId", new { ownerUserId });
            }
        }

        public async Task<bool> ExistsByOwnerAndNameAsync(string ownerUserId, string name)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM [Environment2D] WHERE OwnerUserId = @ownerUserId AND Name = @name", new { ownerUserId, name });
            }
        }
    }
}
