using Indotalent.Applications.AX;
using Indotalent.Data;
using Indotalent.Models.Entities.AX;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MWSManagement.Models.Entities;
using System.Data;

namespace Indotalent.Applications.TableSyncs
{
    public class TableSyncService : AxCrudService<TableSync>
    {
        public TableSyncService(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<string>> GetSystemTablesAsync()
        {
            var tables = new List<string>();

            var conn = _context.Database.GetDbConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }

            return tables;
        }

        public async Task<bool> IsCodeExistsAsync(string code, long currentRecId = 0)
        {
            return await _context.TableSync
                .AnyAsync(x => x.Code == code && x.RecId != currentRecId);
        }
    }
}