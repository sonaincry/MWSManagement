using Indotalent.Applications.Products;
using Indotalent.DTOs;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MWSManagement.DTOs;

namespace Indotalent.Pages.Products
{
    public class ProductDetailModel : BaseActionPageModel
    {
        private readonly ProductService _productService;

        public ProductDetailModel(ProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public ProductDetailDto Product { get; set; } = new();

        public List<string> UnitOptions { get; set; } = new();

        public async Task OnGetAsync(string? productId, string? companyCode)
        {
            InitPageState();

            await LoadUnitOptionsAsync();

            if (IsCreate)
            {
                Product = new ProductDetailDto
                {
                    CompanyCode = "food",
                    SalesPrice = 0
                };

                return;
            }

            if (IsEdit || IsDelete)
            {
                if (string.IsNullOrWhiteSpace(productId) || string.IsNullOrWhiteSpace(companyCode))
                {
                    throw new Exception("Missing productId or companyCode.");
                }

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
            InitPageState();
            ValidateForm();

            await LoadUnitOptionsAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (IsCreate)
                {
                    this.WriteToastMessage("Product created successfully.", "success");

                    return RedirectToPage("/Products/ProductCategoryReport");
                }

                if (IsEdit)
                {
                    await _productService.UpdateProductDetailAsync(Product);

                    this.WriteToastMessage("Success update data.", "success");

                    return RedirectToPage("/Products/ProductList");
                }

                if (IsDelete)
                {
                    //await _productService.DeleteProductAsync(
                    //    Product.ProductId ?? string.Empty,
                    //    Product.CompanyCode ?? string.Empty
                    //);

                    this.WriteToastMessage("Product deleted successfully.", "danger");

                    return RedirectToPage("/Products/ProductList");
                }

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }
        }

        private void ValidateForm()
        {
            if (IsDelete)
            {
                if (string.IsNullOrWhiteSpace(Product.ProductId))
                {
                    ModelState.AddModelError("Product.ProductId", "Product ID is required.");
                }

                if (string.IsNullOrWhiteSpace(Product.CompanyCode))
                {
                    ModelState.AddModelError("Product.CompanyCode", "Company Code is required.");
                }

                return;
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