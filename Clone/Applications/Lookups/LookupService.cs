using Indotalent.Data;
using Microsoft.EntityFrameworkCore;
using MWSManagement.Models.DTOs;
using Dapper;

namespace MWSManagement.Applications.Lookups
{
    public class LookupService
    {
        private readonly ApplicationDbContext _context;

        public LookupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<LookupItem>> GetStoresAsync()
        {
            var sql = @"
                SELECT
                    rst.STORENUMBER AS Value,
                    dpt.NAME AS Text
                FROM RETAILSTORETABLE rst
                JOIN RETAILCHANNELTABLE oct ON oct.RECID = rst.RECID
                JOIN DIRPARTYTABLE dpt ON dpt.RECID = oct.OMOPERATINGUNITID
                ORDER BY dpt.NAME";

            // Get the open DB connection and execute via Dapper
            var conn = _context.Database.GetDbConnection();

            var result = await conn.QueryAsync<LookupItem>(sql);
            return result.ToList();
        }
    }
}