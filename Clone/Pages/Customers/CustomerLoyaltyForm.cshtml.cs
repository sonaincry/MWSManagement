using Indotalent.Applications.Customers;
using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.DTOs.Customers;

namespace Indotalent.Pages.CustomerLoyalty
{
    public class CustomerLoyaltyFormModel : PageModel
    {
        private readonly CustomerService _service;

        public CustomerLoyaltyFormModel(CustomerService service)
        {
            _service = service;
        }

        [BindProperty]
        public CustomerLoyaltyCreateDto Form { get; set; } = new();

        [BindProperty]
        public string? ActionMode { get; set; } = "create";

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string? accountNum, string? action)
        {
            ActionMode = string.IsNullOrWhiteSpace(action)
                ? (!string.IsNullOrEmpty(accountNum) ? "edit" : "create")
                : action;

            if (!string.IsNullOrEmpty(accountNum))
            {
                var existing = await _service.GetCustomerByAccountNumAsync(accountNum);
                if (existing == null)
                {
                    return RedirectToPage("/Customers/CustomerLoyaltyList");
                }

                DateTime? parsedBirthDate = null;
                if (!string.IsNullOrEmpty(existing.BirthDate) && DateTime.TryParse(existing.BirthDate, out var dateVal))
                {
                    parsedBirthDate = dateVal;
                }

                Form = new CustomerLoyaltyCreateDto
                {
                    CustomerAccountNum = existing.AccountNum,
                    CardNumber = existing.Cards,
                    CustomerName = existing.CustomerName,
                    IdentificationNumber = existing.IdentificationNumber,
                    MobilePhone = existing.MobilePhone,
                    Email = existing.Email,
                    FullAddress = existing.FullAddress,
                    BirthDate = parsedBirthDate
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Form.CardNumber) && ActionMode == "create")
            {
                ModelState.AddModelError("Form.CardNumber", "Card Number is required.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                if (ActionMode == "edit")
                {
                    await _service.UpdateCustomerLoyaltyCardAsync(Form);
                    StatusMessage = $"Successfully updated Loyalty Account {Form.CustomerAccountNum}.";
                }
                else
                {
                    var result = await _service.CreateCustomerLoyaltyCardAsync(Form);
                    if (result == null)
                    {
                        ModelState.AddModelError(string.Empty, "Failed to create customer loyalty card.");
                        return Page();
                    }
                    StatusMessage = $"Successfully created Loyalty Card {result.CardNumber} for Account {result.AccountNum}.";
                }

                return RedirectToPage("/Customers/CustomerLoyaltyList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Database Error: " + ex.Message);
                return Page();
            }
        }
    }
}