using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTO.Auth;

public class ChangePasswordRequest
{
    [Required]
    public required string OldPassword { get; set; }
    [Required]
    public required string NewPassword { get; set; }
}