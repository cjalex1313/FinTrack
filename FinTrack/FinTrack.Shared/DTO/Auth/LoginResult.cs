namespace FinTrack.Shared.DTO.Auth;

public class LoginResult
{
    public required string AccessToken { get; set; }
    public bool PasswordSetNeeded { get; set; } = false;
}