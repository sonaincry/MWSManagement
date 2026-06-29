using Microsoft.Data.SqlClient;
using System.Data;

namespace Indotalent.Infrastructures.Repositories
{
    public static class SqlParam
    {
        public static SqlParameter BigInt(string name, long? value)
        {
            return new SqlParameter(name, SqlDbType.BigInt)
            {
                Value = value.HasValue ? value.Value : DBNull.Value
            };
        }

        public static SqlParameter NVarChar(string name, string? value, int size = 50)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, size)
            {
                Value = string.IsNullOrWhiteSpace(value) ? DBNull.Value : value
            };
        }

        public static SqlParameter Int(string name, int? value)
        {
            return new SqlParameter(name, SqlDbType.Int)
            {
                Value = value.HasValue ? value.Value : DBNull.Value
            };
        }

        public static SqlParameter DateTime(string name, DateTime? value)
        {
            return new SqlParameter(name, SqlDbType.DateTime)
            {
                Value = value.HasValue ? value.Value : DBNull.Value
            };
        }

        public static SqlParameter Decimal(string name, decimal? value)
        {
            return new SqlParameter(name, SqlDbType.Decimal)
            {
                Precision = 18,
                Scale = 2,
                Value = value.HasValue ? value.Value : DBNull.Value
            };
        }
    }
}