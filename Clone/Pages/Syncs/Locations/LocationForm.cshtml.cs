using Indotalent.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using MWSManagement.Applications.Locations;
using MWSManagement.Models.Entities;
using Microsoft.Data.SqlClient;

namespace MWSManagement.Pages.Syncs.Locations
{
    public class LocationFormModel : BaseAxCrudPageModel<Location>
    {
        private readonly LocationService _locationService;

        public LocationFormModel(LocationService service) : base(service)
        {
            _locationService = service; 
        }

        public async Task<IActionResult> OnPostTestConnectionAsync([FromBody] ConnectionTestDto config)
        {
            if (config == null) return new JsonResult(new { success = false, message = "No configuration data received." });

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = config.Server,
                InitialCatalog = config.DatabaseName,
                ConnectTimeout = 5
            };

            builder.UserID = config.Username ?? "";
            builder.Password = config.Password ?? "";
            builder.TrustServerCertificate = true;

            try
            {
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync();
                    return new JsonResult(new { success = true, message = "Connection successful!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Connection failed: {ex.Message}" });
            }
        }

        protected override Location CreateDefaultEntity()
        {
            return new Location { };
        }

        protected override void ValidateForm()
        {
            if (ActionMode == "delete") return;

            if (string.IsNullOrWhiteSpace(Form.Name)) ModelState.AddModelError("Form.Name", "Location Name is required.");
            if (string.IsNullOrWhiteSpace(Form.Server)) ModelState.AddModelError("Form.Server", "Server Address is required.");
            if (string.IsNullOrWhiteSpace(Form.DatabaseName)) ModelState.AddModelError("Form.DatabaseName", "Database Name is required.");
            if (string.IsNullOrWhiteSpace(Form.Username)) ModelState.AddModelError("Form.Username", "Username is required.");
            if (string.IsNullOrWhiteSpace(Form.Password)) ModelState.AddModelError("Form.Password", "Password is required.");

            // Đoạn kiểm tra trùng tên Location Name (Đã chạy mượt mà nhờ có _locationService)
            if (!string.IsNullOrWhiteSpace(Form.Name))
            {
                var isExists = _locationService.IsNameExistsAsync(Form.Name, Form.RecId).GetAwaiter().GetResult();
                if (isExists)
                {
                    ModelState.AddModelError("Form.Name", $"The Location Name '{Form.Name}' already exists. Please choose another name.");
                }
            }
        }

        protected override IActionResult RedirectToListPage() => RedirectToPage("/Syncs/Locations/LocationList");
        protected override IActionResult RedirectToEditPage(long recId) => RedirectToPage("/Syncs/Locations/LocationForm", new { recId, action = "edit" });
    }

    public class ConnectionTestDto
    {
        public string Server { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}