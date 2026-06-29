using Microsoft.Data.SqlClient;
using System.Data;

namespace Indotalent.Infrastructures.Repositories
{
    public interface IProcedureRepository
    {
        Task<List<T>> QueryAsync<T>(string sql, params object[] parameters) where T : class;

        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, params object[] parameters) where T : class;

        //Insert, Update, Delete
        Task<int> ExecuteAsync(string sql, params object[] parameters);

        //Return int, decimal, string (object?)
        Task<object?> ExecuteScalarAsync(string sql, params SqlParameter[] parameters);

        Task<DataTable> QueryDataTableAsync(string sql, params SqlParameter[] parameters);
    }
}
