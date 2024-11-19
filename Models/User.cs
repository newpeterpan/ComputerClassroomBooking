public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string AdUsername { get; set; }
    public string Role { get; set; }
    public string Department { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class LoginLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;
    public string LoginStatus { get; set; }
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    
    public User User { get; set; }
} 