using Indotalent.Applications.AX;
using Indotalent.Data;
using Indotalent.Models.Entities.AX;
using Microsoft.EntityFrameworkCore;

namespace Indotalent.Applications.TaxVatNums
{
    public class TaxVatNumTableService : AxCrudService<TaxVatNumTableAX>
    {
        public TaxVatNumTableService(ApplicationDbContext context) : base(context)
        {
        }

        //protected override Task NormalizeAsync(TaxVatNumTableAX input)
        //{
        //    input.CountryRegionId = input.CountryRegionId?.Trim().ToUpper();
        //    input.Name = input.Name?.Trim();
        //    input.VatNum = input.VatNum?.Trim().ToUpper();
        //    input.DataAreaId = input.DataAreaId?.Trim().ToLower();

        //    if (string.IsNullOrWhiteSpace(input.DataAreaId))
        //    {
        //        input.DataAreaId = "food";
        //    }

        //    return Task.CompletedTask;
        //}

        //protected override async Task ValidateBeforeCreateAsync(TaxVatNumTableAX input)
        //{
        //    var exists = await _dbSet.AnyAsync(x =>
        //        x.VatNum == input.VatNum &&
        //        x.DataAreaId == input.DataAreaId);

        //    if (exists)
        //    {
        //        throw new Exception($"VAT number '{input.VatNum}' already exists in company '{input.DataAreaId}'.");
        //    }
        //}

        //protected override async Task ValidateBeforeUpdateAsync(TaxVatNumTableAX input)
        //{
        //    var exists = await _dbSet.AnyAsync(x =>
        //        x.RecId != input.RecId &&
        //        x.VatNum == input.VatNum &&
        //        x.DataAreaId == input.DataAreaId);

        //    if (exists)
        //    {
        //        throw new Exception($"VAT number '{input.VatNum}' already exists in company '{input.DataAreaId}'.");
        //    }
        //}
    }
}