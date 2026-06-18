using Dapper;
using DevExpress.CodeParser;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Indotalent.Data;
using System.Data; // Make sure this is added for IDbConnection

namespace Indotalent.Pages.Products
{
    public class UnitOfMeasureListModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Inject the context directly and keep it
        public UnitOfMeasureListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<UnitOfMeasureDto> UnitList { get; set; } = new();

        [BindProperty]
        public UnitOfMeasureDto NewUnit { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadGridDataAsync();
            return Page();
        }

        private async Task LoadGridDataAsync()
        {
            // Share the existing authenticated connection from Entity Framework
            var conn = _context.Database.GetDbConnection();

            var sql = @"
                SELECT 
                    RECID AS [RecId],
                    SYMBOL AS [Symbol],
                    DECIMALPRECISION AS [DecimalPrecision],
                    UNITOFMEASURECLASS AS [UnitOfMeasureClass],
                    SYSTEMOFUNITS AS [SystemOfUnits]
                FROM [ax].[UNITOFMEASURE]
                ORDER BY SYMBOL ASC;";

            var data = await conn.QueryAsync<UnitOfMeasureDto>(sql);
            UnitList = data.ToList();
        }

        public async Task<IActionResult> OnPostCreateUnitAsync()
        {
            if (string.IsNullOrEmpty(NewUnit.Symbol))
            {
                StatusMessage = "Error: Symbol cannot be empty.";
                return RedirectToPage();
            }

            var conn = _context.Database.GetDbConnection();
            try
            {
                var checkExist = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM [ax].[UNITOFMEASURE] WHERE SYMBOL = @Symbol",
                    new { Symbol = NewUnit.Symbol.Trim() });

                if (checkExist > 0)
                {
                    StatusMessage = $"Error: The unit code '{NewUnit.Symbol}' already exists!";
                    return RedirectToPage();
                }

                long randomRecId = (long)(new Random().NextDouble() * 9_000_000_000_000L) + 1_000_000_000_000L;

                var sqlInsert = @"
                    INSERT INTO [ax].[UNITOFMEASURE] (SYMBOL, DECIMALPRECISION, UNITOFMEASURECLASS, SYSTEMOFUNITS, RECID)
                    VALUES (@Symbol, @DecimalPrecision, @UnitOfMeasureClass, @SystemOfUnits, @RecId);";

                await conn.ExecuteAsync(sqlInsert, new
                {
                    Symbol = NewUnit.Symbol.Trim().ToUpper(),
                    DecimalPrecision = NewUnit.DecimalPrecision,
                    UnitOfMeasureClass = NewUnit.UnitOfMeasureClass,
                    SystemOfUnits = NewUnit.SystemOfUnits,
                    RecId = randomRecId
                });

                StatusMessage = $"Successfully added unit '{NewUnit.Symbol}'.";
            }
            catch (Exception ex)
            {
                StatusMessage = "Database Insertion Error: " + ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateUnitAsync([FromBody] UnitOfMeasureDto updatedUnit)
        {
            var conn = _context.Database.GetDbConnection();
            try
            {
                var sqlUpdate = @"
                    UPDATE [ax].[UNITOFMEASURE]
                    SET DECIMALPRECISION = @DecimalPrecision,
                        UNITOFMEASURECLASS = @UnitOfMeasureClass,
                        SYSTEMOFUNITS = @SystemOfUnits
                    WHERE RECID = @RecId;";

                await conn.ExecuteAsync(sqlUpdate, updatedUnit);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public async Task<IActionResult> OnPostDeleteUnitAsync([FromBody] UnitOfMeasureDto targetUnit)
        {
            var conn = _context.Database.GetDbConnection();
            try
            {
                var sqlDelete = "DELETE FROM [ax].[UNITOFMEASURE] WHERE RECID = @RecId;";
                await conn.ExecuteAsync(sqlDelete, new { RecId = targetUnit.RecId });
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}