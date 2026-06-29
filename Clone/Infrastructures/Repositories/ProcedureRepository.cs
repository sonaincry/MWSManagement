using Indotalent.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using System.Data;

namespace Indotalent.Infrastructures.Repositories
{
    public class ProcedureRepository : IProcedureRepository
    {
        private readonly ApplicationDbContext _context;

        public ProcedureRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<T>> QueryAsync<T>(string sql, params object[] parameters) where T : class
        {
            return await _context.Set<T>()
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>( string sql, params object[] parameters) where T : class
        {
            var data = await _context.Set<T>()
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();

            return data.FirstOrDefault();
        }

        //Return table data
        public async Task<DataTable> QueryDataTableAsync(string sql, params SqlParameter[] parameters)
        {
            var dataTable = new DataTable();

            var connection = (SqlConnection)_context.Database.GetDbConnection();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
            }

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }

        //Return Object: Dec, Int, string
        public async Task<object?> ExecuteScalarAsync(string sql, params SqlParameter[] parameters)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();

            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);
            }

            if (command.Connection!.State != System.Data.ConnectionState.Open)
                await command.Connection.OpenAsync();

            return await command.ExecuteScalarAsync();
        }

        public async Task<int> ExecuteAsync(string sql, params object[] parameters)
        {
            return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }
}