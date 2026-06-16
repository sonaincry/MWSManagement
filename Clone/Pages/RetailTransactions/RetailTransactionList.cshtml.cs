using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.LogSessions
{
    [Authorize]
    public class RetailTransactionListModel : PageModel
    {
        public RetailTransactionListModel() { }

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();
        }

    }
}
