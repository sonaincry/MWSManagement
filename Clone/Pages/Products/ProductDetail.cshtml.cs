using Dapper;
using Indotalent.Applications.Products;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.DTOs;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore; // Added
using Indotalent.Data;               // Added

namespace Indotalent.Pages.Products
{
    public class ProductDetailModel : PageModel
    {
        private readonly ProductService _productService;
        private readonly ApplicationDbContext _context; // Use context directly

        // Inject ApplicationDbContext instead of IConfiguration
        public ProductDetailModel(ProductService productService, ApplicationDbContext context)
        {
            _productService = productService;
            _context = context;
        }

        [BindProperty]
        public ProductDetailDto Product { get; set; } = new();

        public List<string> UnitOptions { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string productId, string companyCode)
        {
            // Fix: Point back to the correct Product list page name if parameters are missing
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(companyCode))
            {
                return RedirectToPage("./ProductCategoryReport");
            }

            var detail = await _productService.GetProductDetailAsync(productId, companyCode);
            if (detail == null)
            {
                return NotFound();
            }

            Product = detail;

            // Borrow the pre-authenticated connection string cleanly
            var conn = _context.Database.GetDbConnection();
            var sql = "SELECT SYMBOL FROM [ax].[UNITOFMEASURE] ORDER BY SYMBOL ASC";
            var units = await conn.QueryAsync<string>(sql);
            UnitOptions = units.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var conn = _context.Database.GetDbConnection();

            // Reload units so dropdown doesn't clear if validation returns Page()
            var sqlUnits = "SELECT SYMBOL FROM [ax].[UNITOFMEASURE] ORDER BY SYMBOL ASC";
            var units = await conn.QueryAsync<string>(sqlUnits);
            UnitOptions = units.ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // 1. Update Company-Specific Sales Price and Unit Selection
                var updateModuleSql = @"
                    UPDATE [ax].[INVENTTABLEMODULE]
                    SET PRICE = @SalesPrice,
                        UNITID = @UnitOfMeasure
                    WHERE ITEMID = @ProductId 
                      AND DATAAREAID = @CompanyCode 
                      AND MODULETYPE = 2;";

                await conn.ExecuteAsync(updateModuleSql, new
                {
                    SalesPrice = Product.SalesPrice,
                    UnitOfMeasure = Product.UnitOfMeasure,
                    ProductId = Product.ProductId,
                    CompanyCode = Product.CompanyCode
                }); // Extracted transaction context since we are on a single connection instance

                // 2. Locate and Update the Global Product Name Reference 
                var updateTranslationSql = @"
                    UPDATE pt
                    SET pt.NAME = @ProductName
                    FROM [ax].[ECORESPRODUCTTRANSLATION] pt
                    INNER JOIN [ax].[INVENTTABLE] i ON i.PRODUCT = pt.PRODUCT
                    WHERE i.ITEMID = @ProductId AND i.DATAAREAID = @CompanyCode;";

                await conn.ExecuteAsync(updateTranslationSql, new
                {
                    ProductName = Product.ProductName,
                    ProductId = Product.ProductId,
                    CompanyCode = Product.CompanyCode
                });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }

            StatusMessage = "Product information successfully synced and updated.";

            // Fix: Explicitly declare the current page path string to stop the browser loop
            return RedirectToPage("./ProductDetail", new { productId = Product.ProductId, companyCode = Product.CompanyCode });
        }
    }
}