using Indotalent.Applications.EcoResProducts;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.EcoResProducts
{
    public class EcoResProductFormModel : PageModel
    {
        private readonly EcoResProductService _service;

        public EcoResProductFormModel(EcoResProductService service)
        {
            _service = service;
        }

        [BindProperty]
        public EcoResProductCreateDto Form { get; set; } = new();

        [BindProperty]
        public EcoResProductPriceDto PriceForm { get; set; } = new();

        public List<EcoResCategoryOptionDto> CategoryOptions { get; set; } = new();
        public List<UnitOfMeasureDto> UnitOptions { get; set; } = new();

        public string ActionMode { get; set; } = "create";

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(long? recId)
        {
            ActionMode = Request.Query["action"].ToString();
            if (string.IsNullOrWhiteSpace(ActionMode)) ActionMode = "create";

            CategoryOptions = await _service.GetCategoryOptionsAsync();
            UnitOptions = await _service.GetUnitOptionsAsync();

            if (ActionMode == "edit" && recId is > 0)
            {
                var existing = await _service.GetByRecIdAsync(recId.Value);
                if (existing == null) throw new Exception($"Unable to load data. RecId={recId}");
                Form = existing;
                PriceForm.ProductRecId = recId.Value;

            
                var currentPrice = await _service.GetCurrentPriceAsync(recId.Value);
                if (currentPrice != null)
                {
                    PriceForm.SalesPrice = currentPrice.Value;
                }
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ActionMode = Request.Query["action"].ToString();
            if (string.IsNullOrWhiteSpace(ActionMode)) ActionMode = "create";

            CategoryOptions = await _service.GetCategoryOptionsAsync();
            UnitOptions = await _service.GetUnitOptionsAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (ActionMode == "edit")
                {
                    await _service.UpdateAsync(Form);
                    StatusMessage = $"Success update product (RecId={Form.RecId}).";
                }
                else
                {
                    var result = await _service.CreateAsync(Form);
                    if (result == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to create product.");
                        return Page();
                    }

                    StatusMessage = $"Success create product {result.NewProductNumber} (RecId={result.NewRecId}).";

                    if (!string.IsNullOrWhiteSpace(PriceForm.UnitId) && PriceForm.SalesPrice > 0)
                    {
                        PriceForm.ProductRecId = result.NewRecId;
                        var priceResult = await _service.SetPriceAsync(PriceForm);
                        StatusMessage += $" Price {priceResult?.SalesPrice:N0} cho ItemId {priceResult?.ItemId}.";
                    }
                }

                return RedirectToPage("/EcoResProducts/EcoResProductList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }
        }


        public async Task<IActionResult> OnPostSetPriceAsync()
        {
            ActionMode = "edit";
            CategoryOptions = await _service.GetCategoryOptionsAsync();
            UnitOptions = await _service.GetUnitOptionsAsync();

            var existing = await _service.GetByRecIdAsync(PriceForm.ProductRecId);
            if (existing == null) throw new Exception("Product not found.");
            Form = existing;

            if (string.IsNullOrWhiteSpace(PriceForm.UnitId))
            {
                ModelState.AddModelError("PriceForm.UnitId", "Please select unit of measures.");
            }
            if (PriceForm.SalesPrice <= 0)
            {
                ModelState.AddModelError("PriceForm.SalesPrice", "Price must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _service.SetPriceAsync(PriceForm);
                StatusMessage = $"Price {result?.SalesPrice:N0} cho ItemId {result?.ItemId}.";
                return RedirectToPage("/EcoResProducts/EcoResProductList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                return Page();
            }
        }
    }
}