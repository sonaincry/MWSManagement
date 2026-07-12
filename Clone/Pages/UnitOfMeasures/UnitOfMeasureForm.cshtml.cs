using Indotalent.Applications.UnitOfMeasures;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.UnitOfMeasures
{
    public class UnitOfMeasureFormModel : PageModel
    {
        private readonly UnitMeasureService _service;

        public UnitOfMeasureFormModel(UnitMeasureService service)
        {
            _service = service;
        }

        [BindProperty]
        public UnitOfMeasureDto Form { get; set; } = new();

        public string ActionMode { get; set; } = "create";

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(long? recId)
        {
            ActionMode = Request.Query["action"].ToString();
            if (string.IsNullOrWhiteSpace(ActionMode)) ActionMode = "create";

            if (ActionMode == "edit" && recId is > 0)
            {
                var list = await _service.GetAllAsync();
                var existing = list.FirstOrDefault(x => x.RecId == recId.Value);
                if (existing == null) throw new Exception($"Unable to load data. RecId={recId}");
                Form = existing;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ActionMode = Request.Query["action"].ToString();
            if (string.IsNullOrWhiteSpace(ActionMode)) ActionMode = "create";

            if (string.IsNullOrWhiteSpace(Form.Symbol))
            {
                ModelState.AddModelError("Form.Symbol", "Unit code is required.");
            }

            if (!ModelState.IsValid) return Page();

            try
            {
                if (ActionMode == "edit")
                {
                    await _service.UpdateAsync(Form);
                    StatusMessage = "Success update unit.";
                }
                else
                {
                    await _service.CreateAsync(Form);
                    StatusMessage = "Success create unit.";
                }
                return RedirectToPage("/UnitOfMeasures/UnitOfMeasureList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }
        }
    }
}