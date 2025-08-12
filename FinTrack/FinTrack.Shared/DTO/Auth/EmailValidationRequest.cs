using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTO.Auth;

public class EmailValidationRequest : BaseRequest
{
    
    public Guid UserId { get; set; }
    [Required]
    public required string Token { get; set; }
}