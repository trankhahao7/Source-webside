namespace PBL3_MicayOnline.Models.DTOs;
public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string? Phone { get; set; }
    public DateTime? CreatedAt { get; set; }

    public int OrderCount { get; set; }
    public int FeedbackCount { get; set; }
}
