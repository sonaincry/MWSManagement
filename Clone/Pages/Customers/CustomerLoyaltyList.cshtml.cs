using Indotalent.Applications.Customers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.ControlUI.Helper.Grids;
using MWSManagement.DTOs.Customers;

namespace Indotalent.Pages.Customers
{
    public class CustomerLoyaltyListModel : PageModel
    {
        private readonly CustomerService _service;

        public CustomerLoyaltyListModel(CustomerService service)
        {
            _service = service;
        }

        public List<CustomerLoyaltyDto> CustomerList { get; set; } = new();
        public List<GridColumnDto> GridColumns { get; set; } = new();

        public async Task OnGetAsync()
        {
            GridColumns = GridColumnHelper.FromModel<CustomerLoyaltyDto>();
            CustomerList = await _service.GetCustomersAsync();
        }

        public async Task<IActionResult> OnGetFilteredAsync(string? accountNumber, string? cardNumber)
        {
            var data = await _service.GetCustomersAsync(accountNumber, cardNumber);
            return new JsonResult(data);
        }

        public async Task<IActionResult> OnPostDeleteAsync([FromBody] List<CustomerLoyaltyDto> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return new JsonResult(new { success = false, message = "No selected row(s)." });
            }

            try
            {
                int deletedCount = 0;
                foreach (var row in rows)
                {
                    if (!string.IsNullOrEmpty(row.AccountNum))
                    {
                        var success = await _service.DeleteCustomerLoyaltyCardAsync(row.AccountNum);
                        if (success) deletedCount++;
                    }
                }

                return new JsonResult(new { success = true, message = $"Deleted {deletedCount} row(s)." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}