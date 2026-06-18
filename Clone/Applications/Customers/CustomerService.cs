using Indotalent.Data;
using Indotalent.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Indotalent.Applications.Customers
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerLoyaltyDto>> GetCustomersAsync(
            string? accountNumber = null,
            string? cardNumber = null)
        {
            var rows = new List<(
                string? AccountNum,
                string? CustomerName,
                string? IdentificationNumber,
                string? MobilePhone,
                string? Email,
                string? BirthDate,
                string? FullAddress,
                string? CardNumber
            )>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "[crt].[GETCUSTOMERLOYALTYCARDS]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@nvc_CustomerAccountNumber",
                string.IsNullOrEmpty(accountNumber) ? DBNull.Value : accountNumber));
            command.Parameters.Add(new SqlParameter("@nvc_CardNumber",
                string.IsNullOrEmpty(cardNumber) ? DBNull.Value : cardNumber));

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                rows.Add((
                    AccountNum: SafeString(reader, "AccountNum"),
                    CustomerName: SafeString(reader, "CustomerName"),
                    IdentificationNumber: SafeString(reader, "IdentificationNumber"),
                    MobilePhone: SafeString(reader, "MobilePhone"),
                    Email: SafeString(reader, "Email"),
                    BirthDate: SafeDate(reader, "BirthDate"),
                    FullAddress: SafeString(reader, "FullAddress"),
                    CardNumber: SafeString(reader, "CardNumber")
                ));
            }

            var result = rows
                .GroupBy(r => r.AccountNum)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .Select(g =>
                {
                    var first = g.First();
                    var cards = g
                        .Where(r => !string.IsNullOrEmpty(r.CardNumber))
                        .Select(r => r.CardNumber!)
                        .Distinct()
                        .ToList();

                    return new CustomerLoyaltyDto
                    {
                        AccountNum = first.AccountNum,
                        CustomerName = first.CustomerName,
                        IdentificationNumber = first.IdentificationNumber,
                        MobilePhone = first.MobilePhone,
                        Email = first.Email,
                        BirthDate = first.BirthDate,
                        FullAddress = first.FullAddress,
                        CardCount = cards.Count,
                        Cards = string.Join(", ", cards)
                    };
                })
                .OrderBy(x => x.AccountNum)
                .ToList();

            return result;
        }

        private static string? SafeString(IDataReader r, string col)
        {
            var i = r.GetOrdinal(col);
            return r.IsDBNull(i) ? null : r.GetValue(i)?.ToString()?.Trim();
        }

        private static string? SafeDate(IDataReader r, string col)
        {
            var i = r.GetOrdinal(col);
            if (r.IsDBNull(i)) return null;
            var dt = Convert.ToDateTime(r.GetValue(i));
            // 1900-01-01 means no birthdate set in AX
            return dt.Year <= 1900 ? null : dt.ToString("yyyy-MM-dd");
        }
    }
}