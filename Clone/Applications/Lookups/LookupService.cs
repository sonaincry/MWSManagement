using Indotalent.Data;
using Microsoft.EntityFrameworkCore;
using MWSManagement.Models.DTOs;

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

            return await _context.Database
                .SqlQueryRaw<LookupItem>(sql)
                .ToListAsync();
        }
    }
}