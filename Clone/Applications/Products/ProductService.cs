using Indotalent.Data;
using Indotalent.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MWSManagement.DTOs;
using System.Data;

namespace Indotalent.Applications.Products
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetProductReportAsync(
            long? categoryRecId = null,
            string? categoryCode = null)
        {
            var list = new List<ProductDto>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "[dbo].[GetProductsWithCategoryReport]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@CategoryRecId",
                categoryRecId.HasValue ? categoryRecId.Value : DBNull.Value));
            command.Parameters.Add(new SqlParameter("@CategoryCode",
                categoryCode != null ? categoryCode : DBNull.Value));

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new ProductDto
                {
                    CompanyCode = Get<string>(reader, "Company Code"),
                    ProductId = Get<string>(reader, "Product ID"),
                    ProductName = Get<string>(reader, "Product Name"),
                    CategoryRecId = GetLong(reader, "Category RecID"),
                    CategoryCode = Get<string>(reader, "Category Code"),
                    CategoryName = Get<string>(reader, "Category Name"),
                    ParentCategoryName = Get<string>(reader, "Parent Category Name"),
                    CategoryHierarchyName = Get<string>(reader, "Category Hierarchy Name"),
                });
            }

            return list;
        }

        public async Task<List<CategoryDTO>> GetCategoriesAsync()
        {
            var list = new List<CategoryDTO>();

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                SELECT 
                    HIER.NAME       AS HierarchyName,
                    CAT.RECID       AS RecId,
                    CAT.CODE        AS Code,
                    CAT.NAME        AS Name,
                    PARENT_CAT.NAME AS ParentName
                FROM [ax].ECORESCATEGORY CAT
                INNER JOIN [ax].ECORESCATEGORYHIERARCHY HIER 
                    ON CAT.CATEGORYHIERARCHY = HIER.RECID
                LEFT JOIN [ax].ECORESCATEGORY PARENT_CAT 
                    ON CAT.PARENTCATEGORY = PARENT_CAT.RECID
                ORDER BY HIER.NAME, CAT.CODE";
            command.CommandType = CommandType.Text;

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new CategoryDTO
                {
                    RecId = GetLong(reader, "RecId") ?? 0,
                    Code = Get<string>(reader, "Code"),
                    Name = Get<string>(reader, "Name"),
                    ParentName = Get<string>(reader, "ParentName"),
                    HierarchyName = Get<string>(reader, "HierarchyName"),
                });
            }

            return list;
        }

        private static T? Get<T>(System.Data.IDataReader r, string col) where T : class
        {
            var ordinal = r.GetOrdinal(col);
            return r.IsDBNull(ordinal) ? null : (T)r.GetValue(ordinal);
        }

        private static long? GetLong(System.Data.IDataReader r, string col)
        {
            var ordinal = r.GetOrdinal(col);
            return r.IsDBNull(ordinal) ? null : Convert.ToInt64(r.GetValue(ordinal));
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(string productId, string companyCode)
        {
            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "[dbo].[GetProductDetailByItem]";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@ItemId", productId));
            command.Parameters.Add(new SqlParameter("@DataAreaId", companyCode));

            if (command.Connection!.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var ordinalPrice = reader.GetOrdinal("Sales Price");
                return new ProductDetailDto
                {
                    ProductId = reader.IsDBNull(reader.GetOrdinal("Product ID")) ? null : reader.GetString(reader.GetOrdinal("Product ID")),
                    ProductName = reader.IsDBNull(reader.GetOrdinal("Product Name")) ? null : reader.GetString(reader.GetOrdinal("Product Name")),
                    ProductDescription = reader.IsDBNull(reader.GetOrdinal("Product Description")) ? null : reader.GetString(reader.GetOrdinal("Product Description")),
                    SalesPrice = reader.IsDBNull(ordinalPrice) ? 0 : reader.GetDecimal(ordinalPrice),
                    UnitOfMeasure = reader.IsDBNull(reader.GetOrdinal("Unit of Measure")) ? null : reader.GetString(reader.GetOrdinal("Unit of Measure")),
                    Barcode = reader.IsDBNull(reader.GetOrdinal("Barcode")) ? null : reader.GetString(reader.GetOrdinal("Barcode")),
                    CompanyCode = reader.IsDBNull(reader.GetOrdinal("Company Code")) ? null : reader.GetString(reader.GetOrdinal("Company Code"))
                };
            }
            return null;
        }
    }
}