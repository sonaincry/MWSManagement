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

        public List<EcoResCategoryOptionDto> CategoryOptions { get; set; } = new();

        public string ActionMode { get; set; } = "create";

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(long? recId)
        {
            ActionMode = Request.Query["action"].ToString();
            if (string.IsNullOrWhiteSpace(ActionMode)) ActionMode = "create";

            CategoryOptions = await _service.GetCategoryOptionsAsync();

            if (ActionMode == "edit" && recId is > 0)
            {
                var existing = await _service.GetByRecIdAsync(recId.Value);
                if (existing == null) throw new Exception($"Unable to load data. RecId={recId}");
                Form = existing;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ActionMode = Request.Query["action"].ToString();
            if (string.IsNullOrWhiteSpace(ActionMode)) ActionMode = "create";

            CategoryOptions = await _service.GetCategoryOptionsAsync();

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
                }

                return RedirectToPage("/EcoResProducts/EcoResProductList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }
        }
    }
}