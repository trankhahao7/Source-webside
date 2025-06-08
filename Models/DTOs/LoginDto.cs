namespace PBL3_MicayOnline.Models.DTOs
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        public string NewPasswordHash { get; set; }
    }
    public class ChangePasswordRequestDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
