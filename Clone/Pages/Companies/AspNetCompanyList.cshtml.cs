using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.Companies
{
    [Authorize]
    public class AspNetCompanyListModel : PageModel
    {
        public AspNetCompanyListModel() { }

        public void OnGet()
        {
            this.SetupViewDataTitleFromUrl();

        }



    }
}
