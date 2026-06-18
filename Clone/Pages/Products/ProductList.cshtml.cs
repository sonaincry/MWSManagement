using Indotalent.Applications.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public List<SelectListItem> CategoryOptions { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadCategoriesAsync();
        }

        // AJAX endpoint: /Reports/ProductCategoryReport?handler=Products&CategoryRecId=xxx
        public async Task<IActionResult> OnGetProductsAsync()
        {
            var data = await _productService.GetProductReportAsync(CategoryRecId, CategoryCode);
            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null  // keep PascalCase to match Syncfusion field names
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
    }
}