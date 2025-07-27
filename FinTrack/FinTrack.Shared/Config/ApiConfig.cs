namespace FinTrack.Shared.Config;

public class ApiConfig
{
    public required AdminConfig AdminConfig { get; set; }
    public required JwtConfig JWTConfig { get; set; }
    public required string EmailConfirmationUrl { get; set; }
    public required string ForgotPasswordUrl { get; set; }
}

public class AdminConfig
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class JwtConfig
{
    public required string Secret { get; set; }
    public required string ValidIssuer { get; set; }
}