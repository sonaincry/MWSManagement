using Indotalent.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MWSManagement.Applications.Locations;
using Microsoft.Data.SqlClient; 

namespace Indotalent.Pages.Syncs
{
    public class LocationManagementModel : PageModel
    {
        private readonly LocationService _locService;

        public LocationManagementModel(LocationService locService)
        {
            _locService = locService;
        }

        public List<LocationDto> Locations { get; set; } = new();

        [BindProperty]
        public LocationDto LocationInput { get; set; } = new();

        public async Task OnGetAsync()
        {
            Locations = await _locService.GetAllLocationsAsync();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (LocationInput.Id == 0)
                await _locService.InsertLocationAsync(LocationInput);
            else
                await _locService.UpdateLocationAsync(LocationInput);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDetailAsync(int id)
        {
            var data = await _locService.GetLocationByIdAsync(id);
            return new JsonResult(data);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _locService.DeleteLocationAsync(id);
            return RedirectToPage();
        }

        // ================= XỬ LÝ AJAX TEST CONNECTION =================
        public async Task<IActionResult> OnPostTestConnectionAsync([FromBody] LocationDto input)
        {
            if (input == null || string.IsNullOrEmpty(input.Server) || string.IsNullOrEmpty(input.DatabaseName))
            {
                return new JsonResult(new { success = false, message = "Vui lòng nhập đầy đủ Server và Database Name!" });
            }

            // Xây dựng connection string động từ thông tin trên Modal UI gửi về
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = input.Server,
                InitialCatalog = input.DatabaseName,
                TrustServerCertificate = true,
                ConnectTimeout = 10 // Giới hạn 10s timeout cho phản hồi nhanh
            };

            if (input.IsWindowsAuth)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.UserID = input.Username ?? "";
                builder.Password = input.Password ?? "";
            }

            try
            {
                using (var conn = new SqlConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync();
                }
                return new JsonResult(new { success = true, message = "Kết nối đến cơ sở dữ liệu thành công!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Kết nối thất bại: {ex.Message}" });
            }
        }
    }
}