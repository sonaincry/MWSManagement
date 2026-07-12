using Indotalent.Applications.UnitOfMeasures;
using Indotalent.DTOs;
using Indotalent.Infrastructures.Repositories;
using Microsoft.Data.SqlClient;
using MWSManagement.DTOs;

namespace Indotalent.Applications.EcoResProducts
{
    public class EcoResProductService
    {
        private readonly IProcedureRepository _repo;
        private readonly UnitMeasureService _unitService;
        public EcoResProductService(IProcedureRepository repo, UnitMeasureService unitService)
        {
            _repo = repo;
            _unitService = unitService;
        }

        public async Task<List<EcoResProductListDto>> GetAllAsync()
        {
            const string sql = @"
                SELECT p.RECID AS RecId,
                       p.DISPLAYPRODUCTNUMBER AS DisplayProductNumber,
                       p.SEARCHNAME AS SearchName,
                       p.PRODUCTTYPE AS ProductType,
                       c.NAME AS CategoryName
                FROM dbo.ECORESPRODUCT p
                LEFT JOIN dbo.ECORESPRODUCTCATEGORY pc ON pc.PRODUCT = p.RECID
                LEFT JOIN dbo.ECORESCATEGORY c ON c.RECID = pc.CATEGORY
                ORDER BY p.RECID DESC";

            return await _repo.QueryAsync<EcoResProductListDto>(sql);
        }

        public async Task<EcoResProductCreateDto?> GetByRecIdAsync(long recId)
        {
            const string sql = @"
                SELECT p.RECID AS RecId,
                       p.SEARCHNAME AS SearchName,
                       p.PRODUCTTYPE AS ProductType,
                       pc.CATEGORY AS CategoryRecId,
                       pc.CATEGORYHIERARCHY AS CategoryHierarchyRecId
                FROM dbo.ECORESPRODUCT p
                LEFT JOIN dbo.ECORESPRODUCTCATEGORY pc ON pc.PRODUCT = p.RECID
                WHERE p.RECID = {0}";

            return await _repo.QueryFirstOrDefaultAsync<EcoResProductCreateDto>(sql, recId);
        }

        public async Task<List<EcoResCategoryOptionDto>> GetCategoryOptionsAsync()
        {
            const string sql = @"
        SELECT RECID AS CategoryRecId,
               CATEGORYHIERARCHY AS CategoryHierarchyRecId,
               NAME AS Name,
               LEVEL_ AS Level,
               NESTEDSETLEFT AS SortOrder
        FROM dbo.ECORESCATEGORY
        WHERE ISACTIVE = 1
        ORDER BY NESTEDSETLEFT";

            return await _repo.QueryAsync<EcoResCategoryOptionDto>(sql);
        }

        public async Task<EcoResProductCreateResultDto?> CreateAsync(EcoResProductCreateDto input)
        {
            const string sql = @"
                EXEC dbo.USP_CreateEcoResProduct
                    @SearchName = {0},
                    @ProductType = {1},
                    @CategoryRecId = {2},
                    @CategoryHierarchyRecId = {3}";

            return await _repo.QueryFirstOrDefaultAsync<EcoResProductCreateResultDto>(
                sql,
                input.SearchName,
                input.ProductType,
                (object?)input.CategoryRecId ?? DBNull.Value,
                (object?)input.CategoryHierarchyRecId ?? DBNull.Value);
        }

        public async Task UpdateAsync(EcoResProductCreateDto input)
        {
            const string sql = @"
                EXEC dbo.USP_UpdateEcoResProduct
                    @RecId = {0},
                    @SearchName = {1},
                    @ProductType = {2},
                    @CategoryRecId = {3},
                    @CategoryHierarchyRecId = {4}";

            await _repo.ExecuteAsync(
                sql,
                input.RecId,
                input.SearchName,
                input.ProductType,
                (object?)input.CategoryRecId ?? DBNull.Value,
                (object?)input.CategoryHierarchyRecId ?? DBNull.Value);
        }

        public async Task<int> DeleteManyAsync(List<EcoResProductListDto> rows)
        {
            if (rows == null || rows.Count == 0) return 0;

            var recIds = rows.Where(r => r.RecId > 0).Select(r => r.RecId).Distinct().ToList();

            foreach (var recId in recIds)
            {
                await _repo.ExecuteAsync("EXEC dbo.USP_DeleteEcoResProduct @RecId = {0}", recId);
            }

            return recIds.Count;
        }

        public async Task<EcoResProductPriceResultDto?> SetPriceAsync(EcoResProductPriceDto input)
        {
            const string sql = @"
        EXEC dbo.USP_ReleaseAndPriceEcoResProduct
            @ProductRecId = {0},
            @UnitId = {1},
            @SalesPrice = {2}";

            return await _repo.QueryFirstOrDefaultAsync<EcoResProductPriceResultDto>(
                sql, input.ProductRecId, input.UnitId, input.SalesPrice);
        }

        public async Task<decimal?> GetCurrentPriceAsync(long productRecId)
        {
            const string sql = @"
        SELECT TOP 1 m.PRICE
        FROM dbo.ECORESPRODUCT p
        INNER JOIN dbo.INVENTTABLE t ON t.PRODUCT = p.RECID
        INNER JOIN dbo.INVENTTABLEMODULE m ON m.ITEMID = t.ITEMID AND m.MODULETYPE = 1
        WHERE p.RECID = @p0";

            var result = await _repo.QueryDataTableAsync(sql, new SqlParameter("@p0", productRecId));
            return result.Rows.Count > 0 ? Convert.ToDecimal(result.Rows[0]["PRICE"]) : (decimal?)null;
        }

        public async Task<List<UnitOfMeasureDto>> GetUnitOptionsAsync()
        {
            return await _unitService.GetAllAsync();
        }
    }
}