using System.DirectoryServices.AccountManagement;
using Microsoft.Extensions.Configuration;

public class AdAuthService
{
    private readonly string _domain;
    private readonly string _container;

    public AdAuthService(IConfiguration configuration)
    {
        _domain = configuration["AD:Domain"];
        _container = configuration["AD:Container"];
    }

    public async Task<AdUserInfo> AuthenticateUserAsync(string username, string password)
    {
        try
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain, _container))
            {
                bool isValid = context.ValidateCredentials(username, password);
                
                if (!isValid)
                    return null;

                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, username);
                
                if (userPrincipal == null)
                    return null;

                return new AdUserInfo
                {
                    Username = userPrincipal.SamAccountName,
                    DisplayName = userPrincipal.DisplayName,
                    Email = userPrincipal.EmailAddress,
                    Department = userPrincipal.GetProperty<string>("department")
                };
            }
        }
        catch (Exception ex)
        {
            throw new AuthenticationException("AD 驗證錯誤", ex);
        }
    }
}

public class AdUserInfo
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
} 