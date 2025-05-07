using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ Cần dòng này để dùng Include
using PBL3_MicayOnline.Data;
using PBL3_MicayOnline.Models;
using PBL3_MicayOnline.Models.DTOs;
[ApiController]
[Route("api/[controller]")]
public class UserApiController : ControllerBase
{
    private readonly Pbl3Context _context;
    public UserApiController(Pbl3Context context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.Orders)
            .Include(u => u.Feedbacks)
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt,
                OrderCount = u.Orders.Count,
                FeedbackCount = u.Feedbacks.Count
            }).ToListAsync();

        return users;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var u = await _context.Users
            .Include(u => u.Orders)
            .Include(u => u.Feedbacks)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (u == null) return NotFound();

        var dto = new UserDto
        {
            UserId = u.UserId,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt,
            OrderCount = u.Orders.Count,
            FeedbackCount = u.Feedbacks.Count
        };

        return dto;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(UserCreateDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            PasswordHash = dto.PasswordHash,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Role = dto.Role,
            CreatedAt = DateTime.Now
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = new UserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            OrderCount = 0,
            FeedbackCount = 0
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserUpdateDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.FullName = dto.FullName ?? user.FullName;
        user.Email = dto.Email ?? user.Email;
        user.Phone = dto.Phone ?? user.Phone;
        user.Role = dto.Role ?? user.Role;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
