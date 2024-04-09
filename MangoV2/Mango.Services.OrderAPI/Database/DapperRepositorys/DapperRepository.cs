using Dapper;
using System.Data;
using Mango.Services.OrderAPI.Database.IDapperRepositorys;

using System.Data.SqlClient;

namespace Mango.Services.OrderAPI.Database.DapperRepositorys
{
    public class DapperRepository: IDapperRepository
    {
        private readonly IDbConnection _dbConnection;

        public DapperRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }


        public async Task<T> GetAsync<T>(string sql, object parameters = null)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string sql, object parameters = null)
        {
            return await _dbConnection.QueryAsync<T>(sql, parameters);
        }

        public async Task<int> ExecuteAsync(string sql, object parameters = null, CommandType? commandType = null)
        {
            return await _dbConnection.ExecuteAsync(sql, parameters, commandType: commandType);
        }

        public async Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedureName, object parameters = null)
        {
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(
            storedProcedureName,
            parameters,
            commandType: CommandType.StoredProcedure
        );
        }
    }
}
