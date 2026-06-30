using Indotalent.Applications.TaxVatNums;
using Indotalent.Models.Entities.AX;
using Indotalent.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Indotalent.Pages.TaxVatNums
{
    public class TaxVatNumTableFormModel : BaseAxCrudPageModel<TaxVatNumTableAX>
    {
        public TaxVatNumTableFormModel(TaxVatNumTableService service) : base(service)
        {
        }

        protected override TaxVatNumTableAX CreateDefaultEntity()
        {
            return new TaxVatNumTableAX
            {
                DataAreaId = "food",
                CountryRegionId = "VNM"
            };
        }

        protected override void ValidateForm()
        {
            if (ActionMode == "delete")
            {
                if (Form.RecId <= 0)
                {
                    ModelState.AddModelError("Form.RecId", "Invalid RecId.");
                }

                return;
            }

            if (string.IsNullOrWhiteSpace(Form.DataAreaId))
            {
                ModelState.AddModelError("Form.DataAreaId", "Company is required.");
            }

            if (string.IsNullOrWhiteSpace(Form.CountryRegionId))
            {
                ModelState.AddModelError("Form.CountryRegionId", "Country is required.");
            }

            if (string.IsNullOrWhiteSpace(Form.VatNum))
            {
                ModelState.AddModelError("Form.VatNum", "VAT Number is required.");
            }

            if (string.IsNullOrWhiteSpace(Form.Name))
            {
                ModelState.AddModelError("Form.Name", "Name is required.");
            }
        }

        protected override IActionResult RedirectToListPage()
        {
            return RedirectToPage("/TaxVatNums/TaxVatNumTableList");
        }

        protected override IActionResult RedirectToEditPage(long recId)
        {
            return RedirectToPage("/TaxVatNums/TaxVatNumTableForm", new
            {
                recId,
                action = "edit"
            });
        }
    }
}