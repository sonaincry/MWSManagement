using Indotalent.Applications.Products;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MWSManagement.ControlUI.Helper.Grids;
using System.Text.Json;

namespace Indotalent.Pages.Products
{
    public class ProductCategoryReportModel : PageModel
    {
        private readonly ProductService _productService;

        public ProductCategoryReportModel(ProductService productService)
        {
            _productService = productService;
        }

        [BindProperty(SupportsGet = true)]
        public long? CategoryRecId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CategoryCode { get; set; }
        public List<GridColumnDto> GridColumns { get; set; } = new();

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<ProductDto>();

            await LoadCategoriesAsync();
        }

        public async Task<IActionResult> OnGetProductsAsync()
        {

            var data = await _productService.GetProductReportAsync(CategoryRecId, CategoryCode);
            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null  
            });
        }

        private async Task LoadCategoriesAsync()
        {
            var cats = await _productService.GetCategoriesAsync();

            CategoryOptions = cats
                .Select(c => new SelectListItem
                {
                    Value = c.RecId.ToString(),
                    Text = string.IsNullOrEmpty(c.Code)
                                ? (c.Name ?? "")
                                : $"{c.Code} – {c.Name}"
                })
                .ToList();

            CategoryOptions.Insert(0, new SelectListItem { Value = "", Text = "— All Categories —" });
        }

        public async Task<IActionResult> OnGetProductDetailAsync(string productId, string companyCode)
        {
            var detail = await _productService.GetProductDetailAsync(productId, companyCode);
            if (detail == null) return NotFound();

            return new JsonResult(detail);
        }

        public async Task<IActionResult> OnPostDeleteProductsAsync([FromBody] List<ProductDto> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = "No selected products."
                });
            }

            try
            {
                var deletedCount = 0;

                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.ProductId) ||
                        string.IsNullOrWhiteSpace(row.CompanyCode))
                    {
                        continue;
                    }

                    //await _productService.DeleteProductAsync(row.ProductId, row.CompanyCode);
                    deletedCount++;
                }

                return new JsonResult(new
                {
                    success = true,
                    message = $"Deleted {deletedCount} product(s)."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}