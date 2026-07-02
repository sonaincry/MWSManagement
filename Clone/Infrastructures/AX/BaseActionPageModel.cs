using Indotalent.Infrastructures.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Indotalent.Pages.Shared
{
    public abstract class BaseActionPageModel : PageModel
    {
        public string ActionMode { get; set; } = "create";

        [TempData]
        public string? StatusMessage { get; set; }

        public bool IsCreate => ActionMode == "create";
        public bool IsEdit => ActionMode == "edit";
        public bool IsDelete => ActionMode == "delete";

        protected void InitPageState()
        {
            this.SetupViewDataTitleFromUrl();
            this.SetupStatusMessage();

            StatusMessage = this.ReadStatusMessage();

            LoadActionMode();
        }

        protected void LoadActionMode()
        {
            ActionMode = Request.Query["action"].ToString();

            if (string.IsNullOrWhiteSpace(ActionMode))
            {
                ActionMode = "create";
            }

            ActionMode = ActionMode.ToLower();
        }

        protected void WriteSuccessMessage(string message)
        {
            this.WriteToastMessage(message, "success");
        }

        protected void WriteErrorMessage(string message)
        {
            this.WriteToastMessage(message, "danger");
        }

        protected void WriteWarningMessage(string message)
        {
            this.WriteToastMessage(message, "warning");
        }

        protected void WriteInfoMessage(string message)
        {
            this.WriteToastMessage(message, "info");
        }
    }
}