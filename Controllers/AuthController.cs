using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AdAuthService _adAuthService;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthController(
        AdAuthService adAuthService,
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        _adAuthService = adAuthService;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var adUser = await _adAuthService.AuthenticateUserAsync(request.Username, request.Password);
            
            if (adUser == null)
                return Unauthorized(new { error = "登入失敗" });

            // 檢查或創建本地用戶記錄
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.AdUsername == request.Username);

            if (user == null)
            {
                // 首次登入，創建用戶記錄
                user = new User
                {
                    Username = adUser.DisplayName,
                    Email = adUser.Email,
                    AdUsername = adUser.Username,
                    Role = "teacher",
                    Department = adUser.Department
                };
                
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();
            }

            // 更新最後登入時間
            user.LastLogin = DateTime.UtcNow;
            
            // 記錄登入日誌
            var loginLog = new LoginLog
            {
                UserId = user.Id,
                LoginStatus = "success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };
            
            _dbContext.LoginLogs.Add(loginLog);
            await _dbContext.SaveChangesAsync();

            // 生成 JWT token
            var token = GenerateJwtToken(user);

            return Ok(new { token, user });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "登入過程發生錯誤" });
        }
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
} 