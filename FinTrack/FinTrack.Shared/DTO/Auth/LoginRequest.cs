namespace FinTrack.Shared.DTO.Auth;

public class LoginRequest : BaseRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}