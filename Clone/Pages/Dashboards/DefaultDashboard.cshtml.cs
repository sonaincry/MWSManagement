using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Indotalent.Pages.Dashboards
{
    [Authorize] // Ensures the user must be logged in to view the dashboard
    public class DefaultDashboardModel : PageModel
    {
        [BindProperty]
        public string StatusMessage { get; set; } = string.Empty;

        // Constructor is completely empty now—no services injected to cause 500 errors!
        public DefaultDashboardModel()
        {
        }

        public void OnGet()
        {
            // Just a blank entry point so the page renders without doing backend work
            StatusMessage = string.Empty;
        }
    }
}