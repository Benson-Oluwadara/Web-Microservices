using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
namespace Mango.Services.CouponAPI.Database.IDapperRepositorys
{
    public interface IDapperRepository
    {
        Task<T> GetAsync<T>(string sql, object parameters = null);
        Task<IEnumerable<T>> GetAllAsync<T>(string sql, object parameters = null);
        Task<int> ExecuteAsync(string sql, object parameters = null, CommandType? commandType = null);


        //Methods for Stored Procedure
        Task<T> ExecuteStoredProcedureAsync<T>(string storedProcedureName, object parameters = null);
        

    }
}
