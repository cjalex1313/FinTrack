namespace FinTrack.Shared.Config;

public class AdminConfig
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class ApiConfig
{
    public required AdminConfig AdminConfig { get; set; }
    public required AuthenticationConfig Authentication { get; set; }
    public required string EmailConfirmationUrl { get; set; }
    public required string ForgotPasswordUrl { get; set; }
    public required JwtConfig JWTConfig { get; set; }
}

public class AuthenticationConfig
{
    public required AuthenticationGoogleConfig Google { get; set; }
}

public class AuthenticationGoogleConfig
{
    public required bool Enabled { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
}
public class JwtConfig
{
    public required string Secret { get; set; }
    public required string ValidIssuer { get; set; }
}

public class HangfireConfig
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}