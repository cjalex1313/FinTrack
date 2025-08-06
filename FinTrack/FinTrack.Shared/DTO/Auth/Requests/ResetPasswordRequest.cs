using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTO.Auth;

public class ResetPasswordRequest
{
    [Required]
    public required string Token { get; set; }
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public required string Password { get; set; }
}