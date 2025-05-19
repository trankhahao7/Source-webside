namespace PBL3_MicayOnline.Models.DTOs
{
    public class ChangePasswordDto
    {
        public int UserId { get; set; }
        public string NewPasswordHash { get; set; }
    }
}
