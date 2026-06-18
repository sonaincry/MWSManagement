using Dapper;
using Indotalent.Data;
using Indotalent.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MWSManagement.Models.DTOs;
using System.Data;

namespace Indotalent.Services
{
    public class SalesReportService
    {
        private readonly ApplicationDbContext _context;

        // Channel map — mở rộng sau khi có nhiều store
        private static readonly Dictionary<string, long> StoreChannelMap = new()
        {
            { "1001", 5637145326L },
            // { "1002", 5637145327L },  thêm store mới vào đây
        };

        private const long AllStoresChannelId = 5637145326L; // tạm thời, sửa sau

        public SalesReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        private long ResolveChannelId(string? storeCode)
        {
            if (string.IsNullOrEmpty(storeCode) || storeCode == "ALL")
                return AllStoresChannelId; // TODO: loop nhiều channel khi có nhiều store
            return StoreChannelMap.TryGetValue(storeCode, out var id) ? id : AllStoresChannelId;
        }

        public async Task<List<SalesByDateReportDTO>> GetSalesByDateAsync(
            string? storeCode,
            DateTime startDate,
            DateTime endDate,
            string? userId = null)
        {
            var channelId = ResolveChannelId(storeCode);

            var sql = "EXEC [crt].[GETSALESBYDATEREPORT] " +
                      "@bi_ChannelId, @dt_StartDate, @dt_EndDate, @nvc_UserId";

            return await _context.Database
                .SqlQueryRaw<SalesByDateReportDTO>(sql,
                    new SqlParameter("@bi_ChannelId", channelId),
                    new SqlParameter("@dt_StartDate", startDate.Date),
                    new SqlParameter("@dt_EndDate",   endDate.Date),
                    new SqlParameter("@nvc_UserId", (object?)userId ?? DBNull.Value))
                .ToListAsync();
        }

        public async Task<List<SalesByStaffReportDTO>> GetSalesByStaffAsync(
    string? storeCode,
    DateTime startDate,
    DateTime endDate,
    string? staffKeyword)
        {
            var channelId = ResolveChannelId(storeCode);

            // Get the raw connection from EF Core
            var conn = _context.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@bi_ChannelId", channelId);
            parameters.Add("@dt_StartDate", startDate.Date);
            parameters.Add("@dt_EndDate", endDate.Date);

            // FIX: Pass standard C# null instead of DBNull.Value for Dapper
            parameters.Add("@nvc_Staff", string.IsNullOrWhiteSpace(staffKeyword) ? null : staffKeyword);

            // Dapper handles the stored procedure and maps loose column names gracefully
            var result = await conn.QueryAsync<SalesByStaffReportDTO>(
                "[crt].[GETSALESBYSTAFFREPORT]",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        public async Task<List<DetailTransactionReportDTO>> GetRetailTransactionsAsync(
            string? transactionId,
            string? receiptId,
            string? store,
            string? terminal,
            DateTime? startDate,
            DateTime? endDate,
            int? type)
        {
            var sql = "EXEC [dbo].[sp_GetRetailTransactions] " +
                      "@TransactionID, @ReceiptID, @Store, @Terminal, @TransDateFrom, @TransDateTo, @Type";

            return await _context.Database
                .SqlQueryRaw<DetailTransactionReportDTO>(sql,
                    new SqlParameter("@TransactionID", (object?)transactionId ?? DBNull.Value),
                    new SqlParameter("@ReceiptID", (object?)receiptId ?? DBNull.Value),
                    new SqlParameter("@Store", (object?)store ?? DBNull.Value),
                    new SqlParameter("@Terminal", (object?)terminal ?? DBNull.Value),
                    new SqlParameter("@TransDateFrom", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : DBNull.Value),
                    new SqlParameter("@TransDateTo", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : DBNull.Value),
                    new SqlParameter("@Type", (object?)type ?? DBNull.Value))
                .ToListAsync();
        }
    }

}