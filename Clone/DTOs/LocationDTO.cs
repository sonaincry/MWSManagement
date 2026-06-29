using System.ComponentModel.DataAnnotations;

namespace Indotalent.DTOs
{
    public class LocationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên Location")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ Server (Ví dụ: NOS\\SQLEXPRESS hoặc 192.168.1.50)")]
        public string Server { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên Database")]
        public string DatabaseName { get; set; } = string.Empty;

        public string? Username { get; set; }
        public string? Password { get; set; }

        // Tính năng tự động bật/tắt Windows Authentication
        public bool IsWindowsAuth { get; set; } = true;

        public string ConnectionString
        {
            get
            {
                if (IsWindowsAuth)
                {
                    return $"Server={Server};Database={DatabaseName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;";
                }
                return $"Server={Server};Database={DatabaseName};User ID={Username};Password={Password};TrustServerCertificate=True;MultipleActiveResultSets=True;";
            }
        }
    }
}