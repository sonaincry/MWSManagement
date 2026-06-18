using Indotalent.Applications.Customers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Indotalect.Pages.Customers
{
    public class CustomerLoyaltyListModel : PageModel
    {
        private readonly CustomerService _customerService;

        public CustomerLoyaltyListModel(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [BindProperty(SupportsGet = true)]
        public string? AccountNumber { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CardNumber { get; set; }

        public void OnGet() { }  // page just renders shell; grid loads via AJAX

        public async Task<IActionResult> OnGetCustomersAsync()
        {
            var data = await _customerService.GetCustomersAsync(AccountNumber, CardNumber);
            return new JsonResult(data, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            });
        }
    }
}