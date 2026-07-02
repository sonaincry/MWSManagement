using Indotalent.Applications.AX;
using Indotalent.Infrastructures.Extensions;
using Indotalent.Models.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.Shared
{
    public abstract class BaseAxCrudPageModel<TEntity> : BaseActionPageModel
        where TEntity : class, IAxEntity, new()
    {
        protected readonly AxCrudService<TEntity> Service;

        protected BaseAxCrudPageModel(AxCrudService<TEntity> service)
        {
            Service = service;
        }

        [BindProperty]
        public TEntity Form { get; set; } = new();

        public virtual async Task OnGetAsync(long? recId)
        {
            InitPageState();

            await LoadLookupsAsync();

            if (ActionMode == "create")
            {
                Form = CreateDefaultEntity();
                return;
            }

            if (ActionMode == "edit" || ActionMode == "delete")
            {
                if (recId == null || recId <= 0)
                {
                    throw new Exception("Invalid RecId.");
                }

                var existing = await Service.GetByRecIdAsync(recId.Value);

                if (existing == null)
                {
                    throw new Exception($"Unable to load data. RecId={recId}");
                }

                Form = existing;
            }
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            InitPageState();

            await LoadLookupsAsync();
            ValidateForm();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (ActionMode == "create")
                {
                    await Service.CreateAsync(Form);
                    this.WriteToastMessage("Success create new data.", "success");
                    return RedirectToListPage();
                }

                if (ActionMode == "edit")
                {
                    await Service.UpdateAsync(Form);
                   
                    this.WriteToastMessage("Success update data.", "success");
                    return RedirectToListPage();
                    //return RedirectToEditPage(Form.RecId);
                }

                if (ActionMode == "delete")
                {
                    await Service.DeleteAsync(Form.RecId);
                    this.WriteToastMessage("Success delete data.", "success");
                    return RedirectToListPage();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }
        }

        protected virtual TEntity CreateDefaultEntity()
        {
            return new TEntity();
        }

        protected virtual Task LoadLookupsAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void ValidateForm()
        {
        }

        protected abstract IActionResult RedirectToListPage();

        protected abstract IActionResult RedirectToEditPage(long recId);
    }
}