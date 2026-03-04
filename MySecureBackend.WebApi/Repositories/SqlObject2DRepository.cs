using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlObject2DRepository : IObject2DRepository
    {
        private readonly string sqlConnectionString;

        public SqlObject2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task InsertAsync(Object2D object2D)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync(
                    "INSERT INTO [Object2D] (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) " +
                    "VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)",
                    object2D);
            }
        }

        public async Task<Object2D?> SelectAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QuerySingleOrDefaultAsync<Object2D>(
                    "SELECT * FROM [Object2D] WHERE Id = @id", new { id });
            }
        }

        public async Task<IEnumerable<Object2D>> SelectAsync()
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Object2D>("SELECT * FROM [Object2D]");
            }
        }

        public async Task<IEnumerable<Object2D>> SelectByEnvironmentAsync(Guid environmentId)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                return await sqlConnection.QueryAsync<Object2D>(
                    "SELECT * FROM [Object2D] WHERE EnvironmentId = @environmentId", new { environmentId });
            }
        }

        public async Task UpdateAsync(Object2D object2D)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync(
                    "UPDATE [Object2D] SET " +
                    "EnvironmentId = @EnvironmentId, " +
                    "PrefabId = @PrefabId, " +
                    "PositionX = @PositionX, " +
                    "PositionY = @PositionY, " +
                    "ScaleX = @ScaleX, " +
                    "ScaleY = @ScaleY, " +
                    "RotationZ = @RotationZ, " +
                    "SortingLayer = @SortingLayer " +
                    "WHERE Id = @Id",
                    object2D);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using (var sqlConnection = new SqlConnection(sqlConnectionString))
            {
                await sqlConnection.ExecuteAsync(
                    "DELETE FROM [Object2D] WHERE Id = @id", new { id });
            }
        }
    }
}
