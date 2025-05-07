namespace PBL3_MicayOnline.Models.DTOs
{
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }  // Lưu ý: nên hash trước khi lưu
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; } = "Customer";

    }
}
