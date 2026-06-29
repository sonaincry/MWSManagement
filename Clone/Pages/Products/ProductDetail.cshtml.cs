using Indotalent.Applications.Products;
using Indotalent.DTOs;
using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.DTOs;
using System.Data;

namespace Indotalent.Pages.Products
{
    public class ProductDetailModel : PageModel
    {
        public string? action { get; set; } = string.Empty;
        private readonly ProductService _productService;

        public ProductDetailModel(ProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public ProductDetailDto Product { get; set; } = new();

        public List<string> UnitOptions { get; set; } = new();

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(string? productId, string? companyCode)
        {
            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();
            StatusMessage = this.ReadStatusMessage();
            action = Request.Query["action"];

            await LoadUnitOptionsAsync();


            if (action == "create")
            {

            }
            else if (action == "edit")
            {
                if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(companyCode))
                {
                    throw new Exception("Missing productId or companyCode.");
                }

                await LoadUnitOptionsAsync();

                var detail = await _productService.GetProductDetailAsync(productId, companyCode);

                if (detail == null)
                {
                    throw new Exception($"Product not found. ProductId={productId}, CompanyCode={companyCode}");
                }

                Product = detail;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Product == null)
            {
                return BadRequest("Product data is empty.");
            }

            if (string.IsNullOrWhiteSpace(Product.ProductId))
            {
                ModelState.AddModelError("Product.ProductId", "Product ID is required.");
            }

            if (string.IsNullOrWhiteSpace(Product.CompanyCode))
            {
                ModelState.AddModelError("Product.CompanyCode", "Company Code is required.");
            }

            if (string.IsNullOrWhiteSpace(Product.ProductName))
            {
                ModelState.AddModelError("Product.ProductName", "Product name is required.");
            }

            if (string.IsNullOrWhiteSpace(Product.UnitOfMeasure))
            {
                ModelState.AddModelError("Product.UnitOfMeasure", "Unit of measure is required.");
            }

            var action = Request.Query["action"].ToString() ?? "create";
            if (action == "create")
            {


            }
            else if (action == "edit")
            {
                try
                {
                    await _productService.UpdateProductDetailAsync(Product);

                    StatusMessage = "Product information successfully updated.";

                    return RedirectToPage("/Products/ProductDetail", new
                    {
                        productId = Product.ProductId,
                        companyCode = Product.CompanyCode
                    });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                    return Page();
                }

            }
            else if (action == "delete")
            {

            }

            return Page();
        }

        private async Task LoadUnitOptionsAsync()
        {
            var units = await _productService.GetUnitOptionsAsync();

            UnitOptions = units
                .Where(x => !string.IsNullOrWhiteSpace(x.Symbol))
                .Select(x => x.Symbol!)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }
    }
}