using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTO.Auth;

public class RegisterRequest : BaseRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}