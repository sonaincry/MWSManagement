using Indotalent.DTOs;
using Indotalent.Infrastructures.Repositories;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using MWSManagement.DTOs;
using System.Data;

namespace Indotalent.Applications.Products
{
    public class ProductService
    {
        private readonly IProcedureRepository _procedureRepository;

        public ProductService(IProcedureRepository procedureRepository)
        {
            _procedureRepository = procedureRepository;
        }

        public async Task<List<ProductDto>> GetProductReportAsync(long? categoryRecId = null, string? categoryCode = null)
        {
            var query = @"EXEC [dbo].[GetProductsWithCategoryReport] 
                            @CategoryRecId, 
                            @CategoryCode";

            return await _procedureRepository.QueryAsync<ProductDto>(
            query,
                SqlParam.BigInt("@CategoryRecId", categoryRecId),
                SqlParam.NVarChar("@CategoryCode", categoryCode, 50)
            );
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(string productId, string companyCode)
        {
            var query = @"EXEC [dbo].[GetProductDetailByItem] 
                            @ProductId, 
                            @CompanyCode";

            return await _procedureRepository.QueryFirstOrDefaultAsync<ProductDetailDto>(
                query,
                SqlParam.NVarChar("@ProductId", productId, 50),
                SqlParam.NVarChar("@CompanyCode", companyCode, 50)
            );
        }

        public async Task<List<UnitOfMeasureDto>> GetUnitOptionsAsync()
        {
            var query = @"
                SELECT * 
                FROM [ax].[UNITOFMEASURE]
                ORDER BY SYMBOL ASC";

            return await _procedureRepository.QueryAsync<UnitOfMeasureDto>(query);
        }

        public async Task<List<CategoryDTO>> GetCategoriesAsync()
        {
            var querry = @"
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

            return await _procedureRepository.QueryAsync<CategoryDTO>(querry);
        }

        public async Task<int> UpdateProductDetailAsync(ProductDetailDto product)
        {
            var query = @"
                UPDATE [ax].[INVENTTABLEMODULE]
                SET PRICE = @SalesPrice,
                    UNITID = @UnitOfMeasure
                WHERE ITEMID = @ProductId
                  AND DATAAREAID = @CompanyCode
                  AND MODULETYPE = 2;

                UPDATE pt
                SET pt.NAME = @ProductName
                FROM [ax].[ECORESPRODUCTTRANSLATION] pt
                INNER JOIN [ax].[INVENTTABLE] i 
                    ON i.PRODUCT = pt.PRODUCT
                WHERE i.ITEMID = @ProductId
                  AND i.DATAAREAID = @CompanyCode;
            ";

            return await _procedureRepository.ExecuteAsync(
                query,
                SqlParam.Decimal("@SalesPrice", product.SalesPrice),
                SqlParam.NVarChar("@UnitOfMeasure", product.UnitOfMeasure),
                SqlParam.NVarChar("@ProductId", product.ProductId),
                SqlParam.NVarChar("@CompanyCode", product.CompanyCode),
                SqlParam.NVarChar("@ProductName", product.ProductName)
            );
        }
    }
}