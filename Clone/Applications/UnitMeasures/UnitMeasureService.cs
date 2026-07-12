using Indotalent.Infrastructures.Repositories;
using Indotalent.DTOs;

namespace Indotalent.Applications.UnitOfMeasures
{
    public class UnitMeasureService
    {
        private readonly IProcedureRepository _repo;

        public UnitMeasureService(IProcedureRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<UnitOfMeasureDto>> GetAllAsync()
        {
            const string sql = @"
                SELECT RECID AS RecId, SYMBOL AS Symbol,
                       DECIMALPRECISION AS DecimalPrecision,
                       UNITOFMEASURECLASS AS UnitOfMeasureClass,
                       SYSTEMOFUNITS AS SystemOfUnits
                FROM dbo.UNITOFMEASURE
                ORDER BY SYMBOL";

            return await _repo.QueryAsync<UnitOfMeasureDto>(sql);
        }

        public async Task<long?> CreateAsync(UnitOfMeasureDto input)
        {
            const string sql = @"
                EXEC dbo.USP_CreateUnitOfMeasure
                    @Symbol = {0}, @DecimalPrecision = {1},
                    @UnitOfMeasureClass = {2}, @SystemOfUnits = {3}";

            var result = await _repo.QueryFirstOrDefaultAsync<RecIdResultDto>(
                sql, input.Symbol, input.DecimalPrecision, input.UnitOfMeasureClass, input.SystemOfUnits);

            return result?.RecId;
        }

        public async Task UpdateAsync(UnitOfMeasureDto input)
        {
            const string sql = @"
                EXEC dbo.USP_UpdateUnitOfMeasure
                    @RecId = {0}, @DecimalPrecision = {1},
                    @UnitOfMeasureClass = {2}, @SystemOfUnits = {3}";

            await _repo.ExecuteAsync(sql, input.RecId, input.DecimalPrecision, input.UnitOfMeasureClass, input.SystemOfUnits);
        }

        public async Task<int> DeleteManyAsync(List<UnitOfMeasureDto> rows)
        {
            if (rows == null || rows.Count == 0) return 0;
            var recIds = rows.Where(r => r.RecId > 0).Select(r => r.RecId).Distinct().ToList();

            foreach (var recId in recIds)
                await _repo.ExecuteAsync("EXEC dbo.USP_DeleteUnitOfMeasure @RecId = {0}", recId);

            return recIds.Count;
        }
    }

    public class RecIdResultDto
    {
        public long RecId { get; set; }
    }
}